using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class MoveConstantTemplateTests : TestCollection
	{
		class MockHitSO : ScriptableObject, IHit
		{
			public Func<GameObject, Vector3?> getPoint = _ => null;

			public Func<Vector3?> TryPoint(GameObject source) {
				return () => this.getPoint(source);
			}

			public Func<T?> Try<T>(GameObject source) where T : Component {
				throw new NotImplementedException();
			}
		}

		class MockMB : MonoBehaviour { }

		[UnityTest]
		public IEnumerator PassAgentToHitter() {
			var calledAgent = null as GameObject;
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
			};
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			hitSO.getPoint = a => { calledAgent = a; return null; };

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.AreSame(agent, calledAgent);
		}

		[UnityTest]
		public IEnumerator MoveRight() {
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
			};
			hitSO.getPoint = _ => Vector3.right;

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			var delta = Time.deltaTime;

			Assert.AreEqual(Vector3.right * delta, agent.transform.position);
		}

		[UnityTest]
		public IEnumerator MoveFromOffCenterRight() {
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
			};
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			hitSO.getPoint = _ => new Vector3(1, 1, 0);
			agent.transform.position = new Vector3(1, 0, 0);

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			var delta = Time.deltaTime;

			Assert.AreEqual(new Vector3(1, delta, 0), agent.transform.position);
		}

		[UnityTest]
		public IEnumerator MoveRightTwice() {
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
			};
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			hitSO.getPoint = _ => Vector3.right;

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			var delta = Time.deltaTime;

			yield return new WaitForEndOfFrame();

			delta += Time.deltaTime;

			Assert.AreEqual(Vector3.right * delta, agent.transform.position);
		}

		[UnityTest]
		public IEnumerator MoveRightTwiceFaster() {
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
				speed = 2f,
			};
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			hitSO.getPoint = _ => Vector3.right;

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			var delta = Time.deltaTime;

			yield return new WaitForEndOfFrame();

			delta += Time.deltaTime;

			Assert.AreEqual(Vector3.right * delta * 2, agent.transform.position);
		}

		[UnityTest]
		public IEnumerator NoMoveWhenNoHit() {
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
				speed = 2f,
			};
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			hitSO.getPoint = _ => null;
			agent.transform.position = Vector3.up;

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			Assert.AreEqual(Vector3.up, agent.transform.position);
		}

		[UnityTest]
		public IEnumerator StopWhenOnTarget() {
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
				speed = float.MaxValue,
			};
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();
			hitSO.getPoint = _ => Vector3.right;

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();

			agent.transform.position = Vector3.zero;

			yield return new WaitForEndOfFrame();

			Assert.AreEqual(Vector3.zero, agent.transform.position);
		}

		class MockPluginSO : ScriptableObject, IModifierFactory
		{
			public Func<GameObject, ModifierFn> getCallbacks = _ => _ => null;

			public ModifierFn GetModifierFnFor(
				GameObject agent
			) => this.getCallbacks(agent);
		}

		[UnityTest]
		public IEnumerator WeightToPluginDataWeight() {
			var data = null as WeightData;
			var hitSO = ScriptableObject.CreateInstance<MockHitSO>();
			var pluginSO = ScriptableObject.CreateInstance<MockPluginSO>();
			var move = new MoveConstant {
				hitter = Reference<IHit>.ScriptableObject(hitSO),
				speed = 1f,
				weight = 0.0112f,
				modifiers = new[] {
					new ModifierData {
						hook = ModifierHook.OnBegin,
						factory = Reference<IModifierFactory>.ScriptableObject(pluginSO),
					},
				},
			};
			var agent = new GameObject();
			var runner = new GameObject().AddComponent<MockMB>();

			hitSO.getPoint = _ => Vector3.right * 100;

			pluginSO.getCallbacks = _ => d => () => data = d.As<WeightData>();

			yield return new WaitForEndOfFrame();

			var routineFn = move.GetRoutineFnFor(agent)!;
			runner.StartCoroutine(routineFn()!.GetEnumerator());

			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			Assert.AreEqual(0.0112f, data!.weight);
		}
	}
}
