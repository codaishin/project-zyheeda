using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsMBTests : TestCollection
{
	class MockTemplateSO : ScriptableObject, IInstructionsTemplate
	{
		public Func<GameObject, InstructionsFunc> getInstructions =
			_ => _ => new YieldInstruction[0];

		public InstructionsFunc GetInstructionsFor(GameObject agent) =>
			this.getInstructions(agent);
	}

	[UnityTest]
	public IEnumerator GetInstructions() {
		var called = 0;
		var agent = new GameObject();
		var instructions = new GameObject().AddComponent<InstructionsMB>();
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();

		instructions.template =
			Reference<IInstructionsTemplate>.ScriptableObject(template);
		instructions.agent =
			agent;

		IEnumerable<YieldInstruction> increment() {
			yield return new WaitForEndOfFrame();
			++called;
		}

		template.getInstructions = _ => _ => increment();

		yield return new WaitForEndOfFrame();

		var enumerator = instructions.GetInstructions(() => true);

		while (enumerator!.MoveNext()) ;

		Assert.AreEqual(1, called);
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

		template.getInstructions = _ => _ => null;

		yield return new WaitForEndOfFrame();

		Assert.Null(instructions.GetInstructions(() => true));
	}

	[UnityTest]
	public IEnumerator GetInstructionsParameters() {
		var calledAgent = null as GameObject;
		var calledRun = null as Func<bool>;
		var agent = new GameObject();
		var run = (Func<bool>)(() => false);
		var instructions = new GameObject().AddComponent<InstructionsMB>();
		var template = ScriptableObject.CreateInstance<MockTemplateSO>();

		instructions.template =
			Reference<IInstructionsTemplate>.ScriptableObject(template);
		instructions.agent =
			agent;

		template.getInstructions = agent => run => {
			calledAgent = agent;
			calledRun = run;
			return new YieldInstruction[0];
		};

		yield return new WaitForEndOfFrame();

		_ = instructions.GetInstructions(run);

		Assert.AreEqual((agent, run), (calledAgent, calledRun));
	}
}
