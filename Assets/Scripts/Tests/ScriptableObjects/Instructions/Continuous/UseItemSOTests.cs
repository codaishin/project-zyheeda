using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UseItemSOTests : TestCollection
{
	class MockAnimationMB : MonoBehaviour, IAnimationStates
	{
		public Action<Animation.State> set = _ => { };

		public void Set(Animation.State state) => this.set(state);
		public void Blend(Animation.BlendState state, float weight) {
			throw new System.NotImplementedException();
		}
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

	class MockItem : IItem
	{
		public Func<Transform, Action?> getUse = _ => null;

		public Animation.State ActiveState { get; set; }
		public float UseAfterSeconds { get; set; }
		public float LeaveActiveStateAfterSeconds { get; set; }
		public Action? GetUseOn(Transform target) => this.getUse(target);
	}

	class MockLoadout : ILoadout
	{
		public IItem? Item { get; set; }
	}

	class MockLoadoutManagerMB : MonoBehaviour, ILoadoutManager
	{
		public ILoadout Current { get; set; } = new MockLoadout {
			Item = new MockItem(),
		};
	}

	[UnityTest]
	public IEnumerator UseHitTransform() {
		var called = null as Transform;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var loadout = agent.AddComponent<MockLoadoutManagerMB>();
		var animation = agent.AddComponent<MockAnimationMB>();
		var target = new GameObject();
		var item = ((MockItem)loadout.Current.Item!);

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => target.transform;
		item.getUse = t => () => called = t;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreSame(target.transform, called);
	}

	[UnityTest]
	public IEnumerator Animate() {
		var called = (Animation.State)(-1);
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var loadout = agent.AddComponent<MockLoadoutManagerMB>();
		var animation = agent.AddComponent<MockAnimationMB>();
		var target = new GameObject();
		var item = ((MockItem)loadout.Current.Item!);

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => target.transform;
		animation.set = s => called = s;

		item.ActiveState = Animation.State.ShootRifle;
		item.getUse = _ => () => { };

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Animation.State.ShootRifle, called);
	}

	[UnityTest]
	public IEnumerator UseComponentsOnAgentChildren() {
		var calledSetWith = (Animation.State)(-1);
		var calledUseWith = null as Transform;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var child = new GameObject();
		var loadout = child.AddComponent<MockLoadoutManagerMB>();
		var animation = child.AddComponent<MockAnimationMB>();
		var target = new GameObject();
		var item = ((MockItem)loadout.Current.Item!);

		child.transform.parent = agent.transform;

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => target.transform;
		animation.set = s => calledSetWith = s;
		item.getUse = t => () => calledUseWith = t;

		item.ActiveState = Animation.State.WalkOrRun;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Animation.State.WalkOrRun, calledSetWith);
		Assert.AreSame(target.transform, calledUseWith);
	}

	[UnityTest]
	public IEnumerator UseAfterSeconds() {
		var called = null as Transform;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var loadout = agent.AddComponent<MockLoadoutManagerMB>();
		var animation = agent.AddComponent<MockAnimationMB>();
		var target = new GameObject();
		var item = ((MockItem)loadout.Current.Item!);

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => target.transform;
		item.getUse = t => () => called = t;

		item.UseAfterSeconds = 0.05f;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.Null(called);

		yield return new WaitForSeconds(0.05f);

		Assert.AreSame(target.transform, called);
	}

	[UnityTest]
	public IEnumerator ResetAnimationAfterSeconds() {
		var called = (Animation.State)(-1);
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var loadout = agent.AddComponent<MockLoadoutManagerMB>();
		var animation = agent.AddComponent<MockAnimationMB>();
		var target = new GameObject();
		var item = ((MockItem)loadout.Current.Item!);

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => target.transform;
		item.getUse = t => () => { };
		animation.set = s => called = s;

		item.LeaveActiveStateAfterSeconds = 0.06f;
		item.ActiveState = Animation.State.ShootRifle;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Animation.State.ShootRifle, called);

		yield return new WaitForSeconds(0.06f);

		Assert.AreEqual(Animation.State.Idle, called);
	}

	[UnityTest]
	public IEnumerator FirstUseThenResetAfterSeconds() {
		var calledSetWith = (Animation.State)(-1);
		var calledUseWith = null as Transform;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var child = new GameObject();
		var loadout = child.AddComponent<MockLoadoutManagerMB>();
		var animation = child.AddComponent<MockAnimationMB>();
		var target = new GameObject();
		var item = ((MockItem)loadout.Current.Item!);

		child.transform.parent = agent.transform;

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => target.transform;
		animation.set = s => calledSetWith = s;
		item.getUse = t => () => calledUseWith = t;

		item.ActiveState = Animation.State.WalkOrRun;
		item.UseAfterSeconds = 0.3f;
		item.LeaveActiveStateAfterSeconds = 0.4f;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(Animation.State.WalkOrRun, null as Transform),
			(calledSetWith, calledUseWith)
		);

		yield return new WaitForSeconds(0.3f);

		Assert.AreEqual(
			(Animation.State.WalkOrRun, target.transform),
			(calledSetWith, calledUseWith)
		);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual(
			(Animation.State.Idle, target.transform),
			(calledSetWith, calledUseWith)
		);
	}

	[UnityTest]
	public IEnumerator FirstResetThenUseAfterSeconds() {
		var calledSetWith = (Animation.State)(-1);
		var calledUseWith = null as Transform;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var child = new GameObject();
		var loadout = child.AddComponent<MockLoadoutManagerMB>();
		var animation = child.AddComponent<MockAnimationMB>();
		var target = new GameObject();
		var item = ((MockItem)loadout.Current.Item!);

		child.transform.parent = agent.transform;

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => target.transform;
		animation.set = s => calledSetWith = s;
		item.getUse = t => () => calledUseWith = t;

		item.ActiveState = Animation.State.WalkOrRun;
		item.UseAfterSeconds = 0.4f;
		item.LeaveActiveStateAfterSeconds = 0.3f;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(Animation.State.WalkOrRun, null as Transform),
			(calledSetWith, calledUseWith)
		);

		yield return new WaitForSeconds(0.3f);

		Assert.AreEqual(
			(Animation.State.Idle, null as Transform),
			(calledSetWith, calledUseWith)
		);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual(
			(Animation.State.Idle, target.transform),
			(calledSetWith, calledUseWith)
		);
	}

	[UnityTest]
	public IEnumerator WhenNoHitDoNotRun() {
		var called = 0;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var child = new GameObject();
		var loadout = child.AddComponent<MockLoadoutManagerMB>();
		var animation = child.AddComponent<MockAnimationMB>();
		var item = ((MockItem)loadout.Current.Item!);

		child.transform.parent = agent.transform;

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => null;
		animation.set = _ => ++called;
		item.getUse = _ => () => ++called;

		item.ActiveState = Animation.State.WalkOrRun;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(0, called);
	}


	[UnityTest]
	public IEnumerator WhenItemCantUseTransformDoNotRun() {
		var called = 0;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var child = new GameObject();
		var loadout = child.AddComponent<MockLoadoutManagerMB>();
		var animation = child.AddComponent<MockAnimationMB>();
		var item = ((MockItem)loadout.Current.Item!);

		child.transform.parent = agent.transform;

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.instructionsSO = instructions;
		runInstructions.agent = agent;

		hitter.tryComponent = _ => new GameObject().transform;
		animation.set = _ => ++called;
		item.getUse = _ => null;

		item.ActiveState = Animation.State.WalkOrRun;

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator NoThrowWhenNoItem() {
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var loadout = agent.AddComponent<MockLoadoutManagerMB>();
		var animation = agent.AddComponent<MockAnimationMB>();

		loadout.Current = new MockLoadout { Item = null };
		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		hitter.tryComponent = _ => new GameObject().transform;

		yield return new WaitForEndOfFrame();

		Assert.DoesNotThrow(() => {
			var runRoutine = instructions.GetInstructionsFor(agent)!;
			foreach (var _ in runRoutine()) ;
		});
	}

	[UnityTest]
	public IEnumerator ThrowWhenHitterNotSet() {
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var agent = new GameObject();
		var loadout = agent.AddComponent<MockLoadoutManagerMB>();
		var animation = agent.AddComponent<MockAnimationMB>();

		runInstructions.agent = agent;
		runInstructions.instructionsSO = instructions;

		yield return new WaitForEndOfFrame();

		// we are good, throw happens during yield
	}

	[UnityTest]
	public IEnumerator HitterCalledWithAgentTransform() {
		var called = null as object;
		var instructions = ScriptableObject.CreateInstance<UseItemSO>();
		var runInstructions = new GameObject().AddComponent<InstructionsMB>();

		var hitter = ScriptableObject.CreateInstance<MockHitterSO>();
		var agent = new GameObject();
		var loadout = agent.AddComponent<MockLoadoutManagerMB>();
		var animation = agent.AddComponent<MockAnimationMB>();

		instructions.hitter = Reference<IHit>.PointToScriptableObject(hitter);
		runInstructions.agent = agent;
		runInstructions.instructionsSO = instructions;

		hitter.tryComponent = t => { called = t; return null; };

		yield return new WaitForEndOfFrame();

		runInstructions.Apply();

		yield return new WaitForEndOfFrame();

		Assert.AreSame(agent.transform, called);
	}
}
