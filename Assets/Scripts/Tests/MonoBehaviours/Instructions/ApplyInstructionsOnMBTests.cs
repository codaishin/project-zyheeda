using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ApplyInstructionsOnMBTests : TestCollection
{
	class MockApplicableMB : MonoBehaviour, IApplicable<InstructionHandleMB>
	{
		public Action<InstructionHandleMB> apply = _ => { };
		public Action<InstructionHandleMB> release = _ => { };
		public void Apply(InstructionHandleMB value) => this.apply(value);
		public void Release(InstructionHandleMB value) => this.release(value);
	}

	[UnityTest]
	public IEnumerator RunApplies() {
		var called = new List<InstructionHandleMB>();
		var runner = new GameObject().AddComponent<InstructionHandleMB>();
		var proxy = new GameObject().AddComponent<ApplyInstructionsOnMB>();
		var applicableArray = new MockApplicableMB[] {
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
		};

		proxy.apply = applicableArray
			.Select(Reference<IApplicable<InstructionHandleMB>>.Component)
			.ToArray();
		proxy.handle = runner;

		yield return new WaitForEndOfFrame();

		applicableArray.ForEach(a => a.apply = r => called.Add(r));

		proxy.Apply();

		CollectionAssert.AreEqual(
			new InstructionHandleMB[] { runner, runner, runner },
			called
		);
	}
	[UnityTest]
	public IEnumerator RunRelease() {
		var called = new List<InstructionHandleMB>();
		var runner = new GameObject().AddComponent<InstructionHandleMB>();
		var proxy = new GameObject().AddComponent<ApplyInstructionsOnMB>();
		var applicableArray = new MockApplicableMB[] {
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
		};

		proxy.apply = applicableArray
			.Select(Reference<IApplicable<InstructionHandleMB>>.Component)
			.ToArray();
		proxy.handle = runner;

		yield return new WaitForEndOfFrame();

		applicableArray.ForEach(a => a.release = r => called.Add(r));

		proxy.Release();

		CollectionAssert.AreEqual(
			new InstructionHandleMB[] { runner, runner, runner },
			called
		);
	}
}
