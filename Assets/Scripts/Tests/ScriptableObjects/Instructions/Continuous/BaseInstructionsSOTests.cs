using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseInstructionsSOTests : TestCollection
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

	class MockInstructionsSO : BaseInstructionsSO<MockInstructions> { }

	[UnityTest]
	public IEnumerator GetInstructionsArguments() {
		var called = (null as GameObject, null as Func<bool>);
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionsSO>();
		var agent = new GameObject();
		var run = (Func<bool>)(() => false);

		instructionsSO.Instructions.getInstructionsFor = (agent, run) => {
			called = (agent, run);
			return () => null;
		};

		yield return new WaitForEndOfFrame();

		_ = instructionsSO.GetInstructionsFor(agent, run);

		Assert.AreEqual((agent, run), called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsFunc() {
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionsSO>();
		var func = (InstructionsFunc)(() => new YieldInstruction[0]);

		instructionsSO.Instructions.getInstructionsFor = (_, __) => func;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(func, instructionsSO.GetInstructionsFor(new GameObject()));
	}
}
