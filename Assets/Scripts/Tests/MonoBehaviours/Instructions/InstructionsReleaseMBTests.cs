using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class InstructionsReleaseMBTests : TestCollection
{
	class MockInstructionsMB : MonoBehaviour, IInstructions
	{
		public IEnumerator<YieldInstruction?>? GetInstructions(Func<bool> run) {
			throw new NotImplementedException();
		}
	}

	class MockInstructionsRunMB : MonoBehaviour, IApplicable<IInstructions>
	{
		public Action<IInstructions> release = _ => { };
		public void Apply(IInstructions value) {
			throw new NotImplementedException();
		}

		public void Release(IInstructions value) => this.release(value);
	}

	[UnityTest]
	public IEnumerator ReleaseInstructions() {
		var releaser = new GameObject().AddComponent<InstructionsReleaseMB>();
		var instructions = new MockInstructionsMB[] {
			new GameObject().AddComponent<MockInstructionsMB>(),
			new GameObject().AddComponent<MockInstructionsMB>(),
			new GameObject().AddComponent<MockInstructionsMB>(),
		};
		var runner = new GameObject().AddComponent<MockInstructionsRunMB>();
		var called = new List<IInstructions>();

		releaser.runner = Reference<IApplicable<IInstructions>>.Component(runner);
		releaser.instructions = instructions
			.Select(Reference<IInstructions>.Component)
			.ToArray();

		runner.release = instructions => called.Add(instructions);

		yield return new WaitForEndOfFrame();

		releaser.Apply();

		CollectionAssert.AreEqual(instructions, called);
	}
}
