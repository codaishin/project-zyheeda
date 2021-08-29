using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

public class AfterStartMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator onStartCalled() {
		var called = 0;
		var startMB = new GameObject("obj").AddComponent<AfterStartMB>();
		startMB.onStart = new UnityEvent();
		startMB.onStart.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator onStartAfterStartCalled() {
		var called = 0;
		var check = (onStart: -1, onAfterStart: -1);
		var startMB = new GameObject("obj").AddComponent<AfterStartMB>();
		startMB.onStart = new UnityEvent();
		startMB.onStart.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		check.onStart = called;

		yield return new WaitForEndOfFrame();

		check.onAfterStart = called;

		Assert.AreEqual((0, 1), check);
	}
}
