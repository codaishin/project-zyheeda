using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEffectRunnerMBTests : TestCollection
{
	private class MockEffectRoutineFactory : IEffectRoutineFactory
	{
		public Finalizable Create(Effect effect) => throw new NotImplementedException();
	}

	private class MockStack : IStack
	{
		public Action<Effect> push = _ => { };

		public IEnumerable<Effect> Effects => throw new NotImplementedException();
		public void Cancel() => throw new NotImplementedException();
		public void Push(Effect effect) => this.push(effect);
	}

	private class MockEffectRunner : BaseEffectRunnerMB<MockEffectRoutineFactory>
	{
		public Func<ConditionStacking, GetStackFunc> getStack
			= (_)
			=> (_, __, ___)
			=> new MockStack();
		public override GetStackFunc GetStack(ConditionStacking stacking) {
			return this.getStack(stacking);
		}
	}

	[UnityTest]
	public IEnumerator PushOnStack() {
		var called = default(Effect);
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		var effect = new Effect();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => new MockStack { push = e => called = e };

		yield return new WaitForEndOfFrame();

		runner.Push(effect);

		Assert.AreSame(effect, called);
	}

	[UnityTest]
	public IEnumerator InitStackCashed() {
		var called = 0;
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => {
			++called;
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		runner.Push(new Effect());
		runner.Push(new Effect());

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator InitStackCashedViaTag() {
		var called = 0;
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => {
			++called;
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		runner.Push(new Effect { tag = EffectTag.Heat });
		runner.Push(new Effect { tag = EffectTag.Physical });

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator InitStackCashedViaStacking() {
		var called = 0;
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => {
			++called;
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		runner.Push(new Effect { stacking = ConditionStacking.Duration });
		runner.Push(new Effect { stacking = ConditionStacking.Intensity });

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator InitStackFormMatchingFactory() {
		var called = default(ConditionStacking);
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = s => (_, __, ___) => {
			called = s;
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		runner.Push(new Effect { stacking = ConditionStacking.Duration });

		Assert.AreEqual(ConditionStacking.Duration, called);
	}

	[UnityTest]
	public IEnumerator CreateStackFedWithRoutineFactory() {
		var fed = default(Func<Effect, Finalizable>);
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (effectToRoutine, __, ___) => {
			fed = effectToRoutine;
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		runner.Push(new Effect());

		Assert.AreEqual(
			(Func<Effect, Finalizable>?)runner.routineFactory.Create,
			fed
		);
	}

	[UnityTest]
	public IEnumerator OnPullStartsCoroutine() {
		var called = 0;
		IEnumerator create() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}
		var routine = new Finalizable(create());
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, onPull, ___) => {
			onPull?.Invoke(routine);
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		runner.Push(new Effect());

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator OnCancelStopsCoroutine() {
		var called = 0;
		IEnumerator create() {
			while (true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}
		var routine = new Finalizable(create());
		var cancel = default(Action<Finalizable>);
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, onPull, onCancel) => {
			onPull?.Invoke(routine);
			cancel = onCancel;
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		runner.Push(new Effect());

		yield return new WaitForFixedUpdate();

		cancel?.Invoke(routine);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(1, called);
	}
}
