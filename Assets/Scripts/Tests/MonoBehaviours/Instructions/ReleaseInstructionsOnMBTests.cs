using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ReleaseInstructionsOnMBTests : TestCollection
{
	class MockApplyMB : MonoBehaviour, IApplicable<InstructionHandleMB>
	{
		public Action<InstructionHandleMB> release = _ => { };

		public void Apply(InstructionHandleMB value) =>
			throw new NotImplementedException();
		public void Release(InstructionHandleMB value) =>
			this.release(value);
	}

	[UnityTest]
	public IEnumerator ReleaseInstructions() {
		var releaser = new GameObject().AddComponent<ReleaseInstructionsOnMB>();
		var instructions = new MockApplyMB[] {
		 new GameObject().AddComponent<MockApplyMB>(),
		 new GameObject().AddComponent<MockApplyMB>(),
		 new GameObject().AddComponent<MockApplyMB>(),
		};
		var handle = new GameObject().AddComponent<InstructionHandleMB>();
		var called =
			new List<(InstructionHandleMB, IApplicable<InstructionHandleMB>)>();

		releaser.handle = handle;
		releaser.instructions = instructions
			.Select(Reference<IApplicable<InstructionHandleMB>>.Component)
			.ToArray();

		instructions[0].release = h => called.Add((h, instructions[0]));
		instructions[1].release = h => called.Add((h, instructions[1]));

		yield return new WaitForEndOfFrame();

		releaser.Apply();

		CollectionAssert.AreEqual(
			new (InstructionHandleMB, IApplicable<InstructionHandleMB>)[] {
				(handle, instructions[0]),
				(handle, instructions[1]),
			},
			called
		);
	}
}
