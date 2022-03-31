using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseInstructionsTemplateMBTests : TestCollection
{
	class MockInstructions : IInstructionsTemplate
	{
		public Func<GameObject, InstructionsFunc> getInstructionsFor =
			(_) => _ => null;

		public InstructionsFunc GetInstructionsFor(GameObject agent) =>
			this.getInstructionsFor(agent);
	}

	class MockInstructionsMB : BaseInstructionsTemplateMB<MockInstructions> { }

	[UnityTest]
	public IEnumerator GetInstructionsArguments() {
		var called = null as GameObject;
		var instructionsMB = new GameObject().AddComponent<MockInstructionsMB>();
		var agent = new GameObject();

		instructionsMB.Template.getInstructionsFor = agent => {
			called = agent;
			return _ => null;
		};

		yield return new WaitForEndOfFrame();

		_ = instructionsMB.GetInstructionsFor(agent);

		Assert.AreEqual(agent, called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsFunc() {
		var instructionsMB = new GameObject().AddComponent<MockInstructionsMB>();
		var func = (InstructionsFunc)(_ => new YieldInstruction[0]);

		instructionsMB.Template.getInstructionsFor = _ => func;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(func, instructionsMB.GetInstructionsFor(new GameObject()));
	}
}
