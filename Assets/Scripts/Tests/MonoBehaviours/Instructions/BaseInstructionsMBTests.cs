using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseInstructionsMBTests : TestCollection
{
	class MockInstructions : IInstructions
	{
		public Func<GameObject, Func<bool>?, InstructionsFunc> getInstructionsFor =
			(_, __) => () => null;

		public InstructionsFunc GetInstructionsFor(
			GameObject agent,
			Func<bool>? run = null
		) => this.getInstructionsFor(agent, run);
	}

	class MockInstructionsMB : BaseInstructionsMB<MockInstructions> { }

	[UnityTest]
	public IEnumerator GetInstructionsArguments() {
		var called = (null as GameObject, null as Func<bool>);
		var instructionsMB = new GameObject().AddComponent<MockInstructionsMB>();
		var agent = new GameObject();
		var run = (Func<bool>)(() => false);

		instructionsMB.Instructions.getInstructionsFor = (agent, run) => {
			called = (agent, run);
			return () => null;
		};

		yield return new WaitForEndOfFrame();

		_ = instructionsMB.GetInstructionsFor(agent, run);

		Assert.AreEqual((agent, run), called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsFunc() {
		var instructionsMB = new GameObject().AddComponent<MockInstructionsMB>();
		var func = (InstructionsFunc)(() => new YieldInstruction[0]);

		instructionsMB.Instructions.getInstructionsFor = (_, __) => func;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(func, instructionsMB.GetInstructionsFor(new GameObject()));
	}
}
