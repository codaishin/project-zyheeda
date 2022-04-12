using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsMBTests : TestCollection
{
	class MockTemplateSO : ScriptableObject, IInstructionsTemplate
	{
		public Func<GameObject, ExternalInstructionsFn> getInstructions =
			_ => () => new InstructionData(new YieldInstruction[0], () => { });

		public ExternalInstructionsFn GetInstructionsFor(GameObject agent) =>
			this.getInstructions(agent);
	}

	[UnityTest]
	public IEnumerator GetInstructionData() {
		var agent = new GameObject();
		var instructions = new GameObject().AddComponent<InstructionsMB>();
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();
		var instructionData = new InstructionData(
			new YieldInstruction[0],
			() => { }
		);

		instructions.template =
			Reference<IInstructionsTemplate>.ScriptableObject(template);
		instructions.agent =
			agent;
		template.getInstructions =
			_ => () => instructionData;

		yield return new WaitForEndOfFrame();

		var got = instructions.GetInstructionData()!.Value;

		Assert.AreEqual(instructionData, got);
	}

	[UnityTest]
	public IEnumerator GetInstructionsNull() {
		var agent = new GameObject();
		var instructions = new GameObject().AddComponent<InstructionsMB>();
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();

		instructions.template =
			Reference<IInstructionsTemplate>.ScriptableObject(template);
		instructions.agent =
			agent;

		template.getInstructions = _ => () => null;

		yield return new WaitForEndOfFrame();

		Assert.False(instructions.GetInstructionData().HasValue);
	}

	[UnityTest]
	public IEnumerator GetInstructionsParameters() {
		var calledAgent = null as GameObject;
		var calledRun = null as Func<bool>;
		var agent = new GameObject();
		var instructions = new GameObject().AddComponent<InstructionsMB>();
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();

		instructions.template =
			Reference<IInstructionsTemplate>.ScriptableObject(template);
		instructions.agent =
			agent;

		template.getInstructions = agent => () => {
			calledAgent = agent;
			return new InstructionData(new YieldInstruction[0], () => { });
		};

		yield return new WaitForEndOfFrame();

		_ = instructions.GetInstructionData();

		Assert.AreSame(agent, calledAgent);
	}
}
