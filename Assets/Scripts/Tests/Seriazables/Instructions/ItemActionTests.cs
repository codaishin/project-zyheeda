using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class ItemActionTests : TestCollection
{
	class MockItem : MonoBehaviour, IApplicable
	{
		public Action apply = () => { };
		public Action release = () => { };

		public void Apply() => this.apply();
		public void Release() => this.release();
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
		var agent = new GameObject().AddComponent<MockItem>();
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
	public IEnumerator Apply() {
		var called = 0;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockItem>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		agent.apply = () => ++called;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator ApplyOnAgentChild() {
		var called = 0;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject();
		var child = new GameObject().AddComponent<MockItem>();
		var target = new GameObject();

		child.transform.parent = agent.transform;

		hitter.tryComponent = _ => target.transform;
		child.apply = () => ++called;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator UseAfterSeconds() {
		var called = null as Transform;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockItem>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		effect.apply = t => called = t;

		useItem.useAfterSeconds = 0.05f;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.Null(called);

		yield return new WaitForSeconds(0.05f);

		Assert.AreSame(target.transform, called);
	}

	[UnityTest]
	public IEnumerator ReleaseAfterSeconds() {
		var calledApply = 0;
		var calledRelease = 0;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			leaveActiveStateAfterSeconds = 0.06f,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockItem>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		agent.apply = () => ++calledApply;
		agent.release = () => ++calledRelease;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, calledApply);

		yield return new WaitForSeconds(0.06f);

		Assert.AreEqual(1, calledRelease);
	}

	[UnityTest]
	public IEnumerator FirstUseThenResetAfterSeconds() {
		var calledApply = 0;
		var calledRelease = 0;
		var calledUseWith = null as Transform;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			useAfterSeconds = 0.3f,
			leaveActiveStateAfterSeconds = 0.4f,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockItem>();
		var target = new GameObject();

		hitter.tryComponent = _ => target.transform;
		agent.apply = () => ++calledApply;
		agent.release = () => ++calledRelease;
		effect.apply = t => calledUseWith = t;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((1, null as Transform), (calledApply, calledUseWith));

		yield return new WaitForSeconds(0.3f);

		Assert.AreSame(target.transform, calledUseWith);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual((1, target.transform), (calledRelease, calledUseWith));
	}

	[UnityTest]
	public IEnumerator FirstResetThenUseAfterSeconds() {
		var calledApply = 0;
		var calledRelease = 0;
		var calledUseWith = null as Transform;
		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var effect = new GameObject().AddComponent<MockEffectMB>();
		var useItem = new ItemAction {
			hitter = Reference<IHit>.ScriptableObject(hitter),
			effect = Reference<IApplicable<Transform>>.Component(effect),
			useAfterSeconds = 0.4f,
			leaveActiveStateAfterSeconds = 0.3f,
		};
		var run = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockItem>();
		var target = new GameObject();

		useItem.hitter = Reference<IHit>.ScriptableObject(hitter);

		hitter.tryComponent = _ => target.transform;
		agent.apply = () => ++calledApply;
		agent.release = () => ++calledRelease;
		effect.apply = t => calledUseWith = t;

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreEqual((1, null as Transform), (calledApply, calledUseWith));

		yield return new WaitForSeconds(0.3f);

		Assert.AreEqual((1, null as Transform), (calledRelease, calledUseWith));

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
		var agent = new GameObject().AddComponent<MockItem>();

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
		var agent = new GameObject().AddComponent<MockItem>();

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
		var agent = new GameObject().AddComponent<MockItem>();

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
		var agent = new GameObject().AddComponent<MockItem>();

		hitter.tryComponent = t => { called = t; return t; };

		yield return new WaitForEndOfFrame();

		var getRoutine = useItem.GetInstructionsFor(agent.gameObject)!;
		run.StartCoroutine(getRoutine()!.GetEnumerator());

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent.transform, called);
	}
}
