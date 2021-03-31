using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEffectRunnerMBTests : TestCollection
{
	private class MockEffectRoutineFactory : IEffectRoutineFactory
	{
		public Finalizable Create(Effect effect)
		{
			throw new NotImplementedException();
		}
	}

	private class MockStack : IStack
	{
		public Func<Effect, Finalizable> effectToRoutine;
		public Action<Finalizable> onPull;
		public Action<Finalizable> onCancel;
		public Action cancel = () => { };
		public Action<Effect> push = _ => { };

		public IEnumerable<Effect> Effects => throw new NotImplementedException();

		public void Cancel()
		{
			throw new NotImplementedException();
		}

		public void Push(Effect effect)
		{
			throw new NotImplementedException();
		}
	}

	private class MockStackFactory : IStackFactory
	{
		public MockStack mockStack = new MockStack();

		public IStack Create(
			Func<Effect, Finalizable> effectToRoutine,
			Action<Finalizable> onPull = default,
			Action<Finalizable> onCancel = default
		) {
			mockStack.effectToRoutine = effectToRoutine;
			mockStack.onCancel = onCancel;
			mockStack.onPull = onPull;
			return mockStack;
		}
	}

	private class MockEffectRunner : BaseEffectRunnerMB<MockEffectRoutineFactory, MockStackFactory> { }

	[UnityTest]
	public IEnumerator InitStack()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Intensity,
				value = new MockStackFactory(),
			},
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Duration,
				value = new MockStackFactory(),
			},
		};

		yield return new WaitForEndOfFrame();

		Assert.True(
			runner[ConditionStacking.Intensity] != null &&
			runner[ConditionStacking.Duration] != null
		);
	}

	[UnityTest]
	public IEnumerator InitStackWithEffectRoutineCreate()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = (ConditionStacking)(22),
				value = new MockStackFactory(),
			},
		};

		yield return new WaitForEndOfFrame();

		var stack = (MockStack)runner[(ConditionStacking)(22)];
		Assert.True(stack.effectToRoutine == runner.routineFactory.Create);
	}

	[UnityTest]
	public IEnumerator InitStackWithOnPullAsCoroutineStart()
	{
		var called = 0;
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Duration,
				value = new MockStackFactory(),
			},
		};

		yield return new WaitForEndOfFrame();

		var stack = (MockStack)runner[ConditionStacking.Duration];
		IEnumerator create() {
			while (true) {
				++called;
				yield return new WaitForFixedUpdate();
			}
		}
		stack.onPull(new Finalizable{ wrapped = create() });

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}


	[UnityTest]
	public IEnumerator InitStackWithOnCanceAsCoroutineStop()
	{
		var called = 0;
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Duration,
				value = new MockStackFactory(),
			},
		};

		yield return new WaitForEndOfFrame();

		var stack = (MockStack)runner[ConditionStacking.Duration];
		IEnumerator create() {
			while (true) {
				++called;
				yield return new WaitForFixedUpdate();
			}
		}
		var routine = new Finalizable{ wrapped = create() };
		stack.onPull(routine);

		yield return new WaitForFixedUpdate();

		stack.onCancel(routine);

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator OnValidateConsolidates()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[0];

		yield return new WaitForEndOfFrame();

		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Intensity,
				value = new MockStackFactory(),
			},
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Intensity,
				value = new MockStackFactory(),
			},
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Duration,
				value = new MockStackFactory(),
			},
		};

		runner.OnValidate();

		CollectionAssert.AreEqual(
			new string[] {
				ConditionStacking.Intensity.ToString(),
				"__duplicate__",
				ConditionStacking.Duration.ToString()
			},
			runner.records.Select(r => r.name)
		);
	}

	[UnityTest]
	public IEnumerator OnValidateReinitStacks()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[0];

		yield return new WaitForEndOfFrame();

		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Intensity,
				value = new MockStackFactory(),
			},
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Intensity,
				value = new MockStackFactory(),
			},
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Duration,
				value = new MockStackFactory(),
			},
		};

		runner.OnValidate();

		var stacks = new IStack[] {
			runner[ConditionStacking.Intensity],
			runner[ConditionStacking.Duration],
		};

		CollectionAssert.AreEqual(
			new MockStack[] {
				runner.records[0].value.mockStack,
				runner.records[2].value.mockStack,
			},
			stacks
		);
	}


	[UnityTest]
	public IEnumerator OnValidateInitStackWithEffectRoutineCreate()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[0];

		yield return new WaitForEndOfFrame();

		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = (ConditionStacking)(22),
				value = new MockStackFactory(),
			},
		};
		runner.OnValidate();

		var stack = (MockStack)runner[(ConditionStacking)(22)];
		Assert.True(stack.effectToRoutine == runner.routineFactory.Create);
	}


	[UnityTest]
	public IEnumerator OnValidateInitStackWithOnPullAsCoroutine()
	{
		var called = 0;
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[0];

		yield return new WaitForEndOfFrame();

		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Duration,
				value = new MockStackFactory(),
			},
		};

		runner.OnValidate();

		var stack = (MockStack)runner[ConditionStacking.Duration];
		IEnumerator create() {
			while (true) {
				++called;
				yield return new WaitForFixedUpdate();
			}
		}
		stack.onPull(new Finalizable{ wrapped = create() });

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}


	[UnityTest]
	public IEnumerator OnValidateInitStackWithOnCanceAsCoroutineStop()
	{
		var called = 0;
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[0];

		yield return new WaitForEndOfFrame();

		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory> {
				key = ConditionStacking.Duration,
				value = new MockStackFactory(),
			},
		};
		runner.OnValidate();

		var stack = (MockStack)runner[ConditionStacking.Duration];
		IEnumerator create() {
			while (true) {
				++called;
				yield return new WaitForFixedUpdate();
			}
		}
		var routine = new Finalizable{ wrapped = create() };
		stack.onPull(routine);

		yield return new WaitForFixedUpdate();

		stack.onCancel(routine);

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, called);
	}

	[UnityTest]
	public IEnumerator OnValidateMarkRecordsWithoutFactoryReference()
	{
		var runner = new GameObject("runner").AddComponent<MockEffectRunner>();
		runner.routineFactory = new MockEffectRoutineFactory();
		runner.records = new Record<ConditionStacking, MockStackFactory>[0];

		yield return new WaitForEndOfFrame();

		runner.records = new Record<ConditionStacking, MockStackFactory>[] {
			new Record<ConditionStacking, MockStackFactory>()
		};

		runner.OnValidate();

		Assert.AreEqual("__no_factory_set__", runner.records[0].name);
	}
}
