using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseInstructionsTemplateSOTests : TestCollection
{
	class MockInstructions : IInstructionsTemplate
	{
		public Func<GameObject, InstructionsFunc> getInstructionsFor =
			_ => _ => null;

		public InstructionsFunc GetInstructionsFor(GameObject agent) =>
			this.getInstructionsFor(agent);
	}

	class MockInstructionsSO : BaseInstructionsTemplateSO<MockInstructions> { }

	[UnityTest]
	public IEnumerator GetInstructionsArguments() {
		var called = null as GameObject;
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionsSO>();
		var agent = new GameObject();

		instructionsSO.Template.getInstructionsFor = agent => {
			called = agent;
			return _ => null;
		};

		yield return new WaitForEndOfFrame();

		_ = instructionsSO.GetInstructionsFor(agent);

		Assert.AreSame(agent, called);
	}

	[UnityTest]
	public IEnumerator GetInstructionsFunc() {
		var instructionsSO = ScriptableObject.CreateInstance<MockInstructionsSO>();
		var func = (InstructionsFunc)(_ => new YieldInstruction[0]);

		instructionsSO.Template.getInstructionsFor = _ => func;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(func, instructionsSO.GetInstructionsFor(new GameObject()));
	}
}
