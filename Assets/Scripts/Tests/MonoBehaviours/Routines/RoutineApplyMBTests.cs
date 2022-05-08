using System;
using System.Collections;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class RoutineApplyMBTests : TestCollection
{
	class MockInstructionsMB : MonoBehaviour, IFactory
	{
		public IRoutine? GetRoutine() {
			throw new NotImplementedException();
		}
	}

	class MockRunMB : MonoBehaviour, IApplicable<IFactory>
	{
		public Action<IFactory> apply = _ => { };
		public Action<IFactory> release = _ => { };

		public void Apply(IFactory value) =>
			this.apply(value);

		public void Release(IFactory value) =>
			this.release(value);
	}

	[UnityTest]
	public IEnumerator Apply() {
		var runner = new GameObject().AddComponent<MockRunMB>();
		var instructions = new GameObject().AddComponent<MockInstructionsMB>();
		var apply = new GameObject().AddComponent<RoutineApplyMB>();
		var called = null as IFactory;

		apply.runner = Reference<IApplicable<IFactory>>.Component(runner);
		apply.routineFactory = Reference<IFactory>.Component(instructions);

		runner.apply = instructions => called = instructions;

		yield return new WaitForEndOfFrame();

		apply.Apply();

		Assert.AreSame(instructions, called);
	}

	[UnityTest]
	public IEnumerator Release() {
		var runner = new GameObject().AddComponent<MockRunMB>();
		var instructions = new GameObject().AddComponent<MockInstructionsMB>();
		var apply = new GameObject().AddComponent<RoutineApplyMB>();
		var called = null as IFactory;

		apply.runner = Reference<IApplicable<IFactory>>.Component(runner);
		apply.routineFactory = Reference<IFactory>.Component(instructions);

		runner.release = instructions => called = instructions;

		yield return new WaitForEndOfFrame();

		apply.Release();

		Assert.AreSame(instructions, called);
	}
}
