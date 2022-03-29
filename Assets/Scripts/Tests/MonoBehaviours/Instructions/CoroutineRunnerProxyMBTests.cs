using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CoroutineRunnerProxyMBTests : TestCollection
{
	class MockApplicableMB : MonoBehaviour, IApplicable<CoroutineRunnerMB>
	{
		public Action<CoroutineRunnerMB> apply = _ => { };
		public Action<CoroutineRunnerMB> release = _ => { };
		public void Apply(CoroutineRunnerMB value) => this.apply(value);
		public void Release(CoroutineRunnerMB value) => this.release(value);
	}

	[UnityTest]
	public IEnumerator RunApplies() {
		var called = new List<CoroutineRunnerMB>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		var proxy = new GameObject().AddComponent<CoroutineRunnerProxyMB>();
		var applicableArray = new MockApplicableMB[] {
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
		};

		proxy.apply = applicableArray
			.Select(Reference<IApplicable<CoroutineRunnerMB>>.Component)
			.ToArray();
		proxy.runner = runner;

		yield return new WaitForEndOfFrame();

		applicableArray.ForEach(a => a.apply = r => called.Add(r));

		proxy.Apply();

		CollectionAssert.AreEqual(
			new CoroutineRunnerMB[] { runner, runner, runner },
			called
		);
	}
	[UnityTest]
	public IEnumerator RunRelease() {
		var called = new List<CoroutineRunnerMB>();
		var runner = new GameObject().AddComponent<CoroutineRunnerMB>();
		var proxy = new GameObject().AddComponent<CoroutineRunnerProxyMB>();
		var applicableArray = new MockApplicableMB[] {
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
			new GameObject().AddComponent<MockApplicableMB>(),
		};

		proxy.apply = applicableArray
			.Select(Reference<IApplicable<CoroutineRunnerMB>>.Component)
			.ToArray();
		proxy.runner = runner;

		yield return new WaitForEndOfFrame();

		applicableArray.ForEach(a => a.release = r => called.Add(r));

		proxy.Release();

		CollectionAssert.AreEqual(
			new CoroutineRunnerMB[] { runner, runner, runner },
			called
		);
	}

	[UnityTest]
	public IEnumerator ThrowWhenNoRunnerAssigned() {
		var proxy = new GameObject().AddComponent<CoroutineRunnerProxyMB>();

		yield return new WaitForEndOfFrame();

		LogAssert.Expect(LogType.Error, $"{proxy}: requires runner to be set");
	}
}
