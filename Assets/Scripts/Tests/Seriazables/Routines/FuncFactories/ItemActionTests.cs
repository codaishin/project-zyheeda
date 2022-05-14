using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class ItemActionTests : TestCollection
	{
		class MockHitterSO : ScriptableObject, IHit
		{
			public Func<GameObject, Component?> tryComponent = _ => null;

			public Func<T?> Try<T>(GameObject source) where T : Component {
				return () => (T?)(this.tryComponent(source) ?? null);
			}

			public Func<Vector3?> TryPoint(GameObject source) {
				throw new NotImplementedException();
			}
		}

		class MockPluginSO : ScriptableObject, IModifierFactory
		{
			public Action<RoutineData> action = _ => { };

			public ModifierFn GetModifierFnFor(GameObject agent) {
				return d => () => this.action(d);
			}
		}

		class MockEffectMB : MonoBehaviour, IApplicable<Transform>
		{
			public Action<Transform> apply = _ => { };
			public void Apply(Transform value) => this.apply(value);
			public void Release(Transform value) { }
		}

		class MockMB : MonoBehaviour { }

		[UnityTest]
		public IEnumerator ApplyHitTransform() {
			var called = null as Transform;
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
				effect = Reference<IApplicable<Transform>>.Component(effect),
			};
			var run = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject();
			var target = new GameObject();

			useItem.hitter = Reference<IHit>.ScriptableObject(hitter);
			hitter.tryComponent = _ => target.transform;
			effect.apply = t => called = t;

			yield return new WaitForEndOfFrame();

			var routineFn = useItem.GetRoutineFnFor(agent)!;
			run.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.AreSame(target.transform, called);
		}

		[UnityTest]
		public IEnumerator PassHitTransformToPluginData() {
			var called = null as Transform;
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var plugin = ScriptableObject.CreateInstance<MockPluginSO>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
				effect = Reference<IApplicable<Transform>>.Component(effect),
				modifiers = new[] {
					new ModifierData {
						hook = ModifierHook.OnBegin,
						factory = Reference<IModifierFactory>.ScriptableObject(plugin),
					},
				},
			};
			var run = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject();
			var target = new GameObject();

			useItem.hitter = Reference<IHit>.ScriptableObject(hitter);
			hitter.tryComponent = _ => target.transform;
			plugin.action = d => called = d.As<TargetData>()!.target;

			yield return new WaitForEndOfFrame();

			var routineFn = useItem.GetRoutineFnFor(agent)!;
			run.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.AreSame(target.transform, called);
		}

		[UnityTest]
		public IEnumerator ApplyHitTransformAfterPreCastSeconds() {
			var called = null as Transform;
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
				effect = Reference<IApplicable<Transform>>.Component(effect),
			};
			var run = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject();
			var target = new GameObject();

			useItem.hitter = Reference<IHit>.ScriptableObject(hitter);
			useItem.preCastSeconds = 0.5f;
			hitter.tryComponent = _ => target.transform;
			effect.apply = t => called = t;

			yield return new WaitForEndOfFrame();

			var routineFn = useItem.GetRoutineFnFor(agent)!;
			run.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.Null(called);

			yield return new WaitForSeconds(0.5f);

			Assert.AreSame(target.transform, called);
		}

		[UnityTest]
		public IEnumerator LastForAftercast() {
			var called = 0;
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var plugin = ScriptableObject.CreateInstance<MockPluginSO>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
				effect = Reference<IApplicable<Transform>>.Component(effect),
				modifiers = new[] {
					new ModifierData {
						hook = ModifierHook.OnEnd,
						factory = Reference<IModifierFactory>.ScriptableObject(plugin),
					},
				},
			};
			var run = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject();
			var target = new GameObject();

			useItem.hitter = Reference<IHit>.ScriptableObject(hitter);
			useItem.afterCastSeconds = 0.4f;
			hitter.tryComponent = _ => target.transform;

			plugin.action = _ => ++called;

			yield return new WaitForEndOfFrame();

			var routineFn = useItem.GetRoutineFnFor(agent)!;
			run.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.AreEqual(0, called);

			yield return new WaitForSeconds(0.4f);

			Assert.AreEqual(1, called);
		}

		[UnityTest]
		public IEnumerator UseFrameUpdate() {
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
				effect = Reference<IApplicable<Transform>>.Component(effect),
			};
			var run = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject();
			var target = new GameObject();

			useItem.hitter = Reference<IHit>.ScriptableObject(hitter);
			hitter.tryComponent = _ => target.transform;

			yield return new WaitForEndOfFrame();

			var routineFn = useItem.GetRoutineFnFor(agent)!;

			CollectionAssert.AllItemsAreInstancesOfType(
				routineFn!(),
				typeof(WaitForEndOfFrame)
			);
		}

		[UnityTest]
		public IEnumerator WhenNoHitReturnNull() {
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
				effect = Reference<IApplicable<Transform>>.Component(effect),
			};
			var agent = new GameObject();

			hitter.tryComponent = _ => null;

			yield return new WaitForEndOfFrame();

			var routineFn = useItem.GetRoutineFnFor(agent)!;

			Assert.Null(routineFn!());
		}

		[Test]
		public void ThrowWhenHitterNotSet() {
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var useItem = new ItemAction {
				effect = Reference<IApplicable<Transform>>.Component(effect),
			};
			var agent = new GameObject();

			Assert.Throws<NullReferenceException>(
				() => _ = useItem.GetRoutineFnFor(agent)!
			);
		}

		[Test]
		public void ThrowWhenEffectNotSet() {
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
			};
			var agent = new GameObject();

			Assert.Throws<NullReferenceException>(
				() => _ = useItem.GetRoutineFnFor(agent)!
			);
		}

		[UnityTest]
		public IEnumerator HitterCalledWithAgentTransform() {
			var called = null as GameObject;
			var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
			var effect = new GameObject().AddComponent<MockEffectMB>();
			var useItem = new ItemAction {
				hitter = Reference<IHit>.ScriptableObject(hitter),
				effect = Reference<IApplicable<Transform>>.Component(effect),
			};
			var run = new GameObject().AddComponent<MockMB>();
			var agent = new GameObject();

			hitter.tryComponent = t => { called = t; return t.transform; };

			yield return new WaitForEndOfFrame();

			var routineFn = useItem.GetRoutineFnFor(agent)!;
			run.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.AreSame(agent, called);
		}
	}
}
