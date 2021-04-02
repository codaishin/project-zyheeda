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
		public IEnumerable<Effect> Effects => throw new NotImplementedException();
		public void Cancel() => throw new NotImplementedException();
		public void Push(Effect effect) => throw new NotImplementedException();
	}

	private class MockEffectRunner : BaseEffectRunnerMB<MockEffectRoutineFactory>
	{
		public Func<ConditionStacking, GetStackFunc> getStack;
		public override GetStackFunc GetStack(ConditionStacking stacking) => this.getStack(stacking);
	}

	[UnityTest]
	public IEnumerator InitStack()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		var stack = new MockStack();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => stack;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(stack, runner[EffectTag.Heat, ConditionStacking.Intensity]);
	}

	[UnityTest]
	public IEnumerator InitStackCashed()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => new MockStack();

		yield return new WaitForEndOfFrame();

		Assert.AreSame(
			runner[EffectTag.Heat, ConditionStacking.Intensity],
			runner[EffectTag.Heat, ConditionStacking.Intensity]
		);
	}

	[UnityTest]
	public IEnumerator InitStackCashedViaTag()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => new MockStack();

		yield return new WaitForEndOfFrame();

		Assert.AreNotSame(
			runner[EffectTag.Heat, ConditionStacking.Intensity],
			runner[EffectTag.Physical, ConditionStacking.Intensity]
		);
	}

	[UnityTest]
	public IEnumerator InitStackCashedViaStacking()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, __, ___) => new MockStack();

		yield return new WaitForEndOfFrame();

		Assert.AreNotSame(
			runner[EffectTag.Heat, ConditionStacking.Intensity],
			runner[EffectTag.Heat, ConditionStacking.Duration]
		);
	}

	[UnityTest]
	public IEnumerator InitStackFormMatchingFactory()
	{
		var called = default(ConditionStacking);
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = s => (_, __, ___) => {
			called = s;
			return new MockStack();
		};

		yield return new WaitForEndOfFrame();

		_ = runner[EffectTag.Heat, ConditionStacking.Duration];

		Assert.AreEqual(ConditionStacking.Duration, called);
	}

	[UnityTest]
	public IEnumerator CreateStackFedWithRoutineFactory()
	{
		var fed = default(Func<Effect, Finalizable>);
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (effectToRoutine, __, ___) => {
			fed = effectToRoutine;
			return default;
		};

		yield return new WaitForEndOfFrame();

		_ = runner[EffectTag.Heat, ConditionStacking.Intensity];

		Assert.True(runner.routineFactory.Create == fed);
	}

	[UnityTest]
	public IEnumerator OnPullStartsCoroutine()
	{
		var called = 0;
		IEnumerator create() {
			while(true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}
		var routine = new Finalizable{ wrapped = create() };
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, onPull, ___) => {
			onPull?.Invoke(routine);
			return default;
		};

		yield return new WaitForEndOfFrame();

		_ = runner[EffectTag.Heat, ConditionStacking.Duration];

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(3, called);
	}

	[UnityTest]
	public IEnumerator OnCancelStopsCoroutine()
	{
		var called = 0;
		IEnumerator create() {
			while(true) {
				yield return new WaitForFixedUpdate();
				++called;
			}
		}
		var routine = new Finalizable{ wrapped = create() };
		var cancel = default(Action<Finalizable>);
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.getStack = _ => (_, onPull, onCancel) => {
			onPull?.Invoke(routine);
			cancel = onCancel;
			return default;
		};

		yield return new WaitForEndOfFrame();

		_ = runner[EffectTag.Heat, ConditionStacking.Duration];

		yield return new WaitForFixedUpdate();

		cancel?.Invoke(routine);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual(1, called);
	}
}
