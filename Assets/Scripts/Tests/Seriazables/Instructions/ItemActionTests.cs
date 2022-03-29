using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class ItemActionTests : TestCollection
{
	class MockAnimationMB : MonoBehaviour, IAnimationStates
	{
		public Action<Animation.State> set = _ => { };
		public void Set(Animation.State state) => this.set(state);
	}

	class MockHitterSO : ScriptableObject, IHit
	{
		public Func<object, object?> tryComponent = _ => null;

		public T? Try<T>(T source) where T : Component {
			return (T?)(this.tryComponent(source) ?? null);
		}

		public Vector3? TryPoint(Transform source) {
			throw new System.NotImplementedException();
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
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var target = new GameObject();

		useItem.hitter = Reference<IHit>.ScriptableObject(hitter);
		hitter.tryComponent = _ => target.transform;
		effect.apply = t => called = t;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreSame(target.transform, called);
	}

	[UnityTest]
	public IEnumerator AnimationState() {
		var called = (Animation.State)(-1);
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			activeState = Animation.State.ShootRifle,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		agent.set = s => called = s;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Animation.State.ShootRifle, called);
	}

	[UnityTest]
	public IEnumerator ApplyOnAgentChild() {
		var called = (Animation.State)(-1);
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			activeState = Animation.State.ShootRifle,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject();
		var child = new GameObject().AddComponent<MockAnimationMB>();
		var target = new GameObject();

		child.transform.parent = agent.transform;
		hitter.tryComponent = _ => target.transform;
		child.set = s => called = s;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Animation.State.ShootRifle, called);
	}

	[UnityTest]
	public IEnumerator UseAfterSeconds() {
		var called = null as Transform;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			activeState = Animation.State.ShootRifle,
			useAfterSeconds = 0.05f,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		effect.apply = t => called = t;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.Null(called);

		yield return new WaitForSeconds(0.05f);

		Assert.AreSame(target.transform, called);
	}

	[UnityTest]
	public IEnumerator ResetStateAfterSeconds() {
		var called = (Animation.State)(-1);
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			leaveActiveStateAfterSeconds = 0.06f,
			activeState = Animation.State.ShootRifle,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		agent.set = s => called = s;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Animation.State.ShootRifle, called);

		yield return new WaitForSeconds(0.06f);

		Assert.AreEqual(Animation.State.Idle, called);
	}

	[UnityTest]
	public IEnumerator FirstUseThenResetAfterSeconds() {
		var calledState = (Animation.State)(-1);
		var calledUseWith = null as Transform;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			useAfterSeconds = 0.3f,
			leaveActiveStateAfterSeconds = 0.4f,
			activeState = Animation.State.WalkOrRun,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		agent.set = s => calledState = s;
		effect.apply = t => calledUseWith = t;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(Animation.State.WalkOrRun, null as Transform),
			(calledState, calledUseWith)
		);

		yield return new WaitForSeconds(0.3f);

		Assert.AreSame(target.transform, calledUseWith);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual(
			(Animation.State.Idle, target.transform),
			(calledState, calledUseWith)
		);
	}

	[UnityTest]
	public IEnumerator FirstResetThenUseAfterSeconds() {
		var calledState = (Animation.State)(-1);
		var calledUseWith = null as Transform;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			useAfterSeconds = 0.4f,
			leaveActiveStateAfterSeconds = 0.3f,
			activeState = Animation.State.ShootRifle,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockAnimationMB>();
		var target = new GameObject();

		useItem.hitter = Reference<IHit>.ScriptableObject(hitter);

		hitter.tryComponent = _ => target.transform;
		agent.set = s => calledState = s;
		effect.apply = t => calledUseWith = t;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(Animation.State.ShootRifle, null as Transform),
			(calledState, calledUseWith)
		);

		yield return new WaitForSeconds(0.3f);

		Assert.AreEqual(
			(Animation.State.Idle, null as Transform),
			(calledState, calledUseWith)
		);

		yield return new WaitForSeconds(0.1f);

		Assert.AreSame(target.transform, calledUseWith);
	}

	[UnityTest]
	public IEnumerator WhenNoHitReturnNull() {
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
		};
		var agent = new GameObject().AddComponent<MockAnimationMB>();

		hitter.tryComponent = _ => null;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject);
		Assert.Null(getRoutine());
	}

	[Test]
	public void ThrowWhenHitterNotSet() {
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			effect = Reference<IApplicable<Transform>>.Component(effect),
		};
		var agent = new GameObject().AddComponent<MockAnimationMB>();

		Assert.Throws<NullReferenceException>(
			() => _ = useItem.GetInstructionsFor(agent.gameObject)
		);
	}

	[Test]
	public void ThrowWhenEffectNotSet() {
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
		};
		var agent = new GameObject().AddComponent<MockAnimationMB>();

		Assert.Throws<NullReferenceException>(
			() => _ = useItem.GetInstructionsFor(agent.gameObject)
		);
	}
	[Test]
	public void ThrowWhenAgentHasNoIAnimationStates() {
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
		};
		var agent = new GameObject();

		Assert.Throws<MissingComponentException>(
			() => _ = useItem.GetInstructionsFor(agent)
		);
	}

	[UnityTest]
	public IEnumerator HitterCalledWithAgentTransform() {
		var called = null as object;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockAnimationMB>();

		hitter.tryComponent = t => { called = t; return t; };

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent.transform, called);
	}
}
