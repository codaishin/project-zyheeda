using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsDispatchMBTests : TestCollection
{
	class MockInstructionsMB : MonoBehaviour, IInstructions
	{
		public InstructionData? GetInstructionData() {
			throw new NotImplementedException();
		}
	}

	class MockRunMB : MonoBehaviour, IApplicable<IInstructions>
	{
		public Action<IInstructions> apply = _ => { };
		public Action<IInstructions> release = _ => { };

		public void Apply(IInstructions value) =>
			this.apply(value);

		public void Release(IInstructions value) =>
			this.release(value);
	}

	[UnityTest]
	public IEnumerator Apply() {
		var runner = new GameObject().AddComponent<MockRunMB>();
		var instructions = new GameObject().AddComponent<MockInstructionsMB>();
		var dispatch = new GameObject().AddComponent<InstructionsDispatchMB>();
		var called = null as IInstructions;

		dispatch.runner = Reference<IApplicable<IInstructions>>.Component(runner);
		dispatch.instructions = Reference<IInstructions>.Component(instructions);

		runner.apply = instructions => called = instructions;

		yield return new WaitForEndOfFrame();

		dispatch.Apply();

		Assert.AreSame(instructions, called);
	}

	[UnityTest]
	public IEnumerator Release() {
		var runner = new GameObject().AddComponent<MockRunMB>();
		var instructions = new GameObject().AddComponent<MockInstructionsMB>();
		var dispatch = new GameObject().AddComponent<InstructionsDispatchMB>();
		var called = null as IInstructions;

		dispatch.runner = Reference<IApplicable<IInstructions>>.Component(runner);
		dispatch.instructions = Reference<IInstructions>.Component(instructions);

		runner.release = instructions => called = instructions;

		yield return new WaitForEndOfFrame();

		dispatch.Release();

		Assert.AreSame(instructions, called);
	}

}
