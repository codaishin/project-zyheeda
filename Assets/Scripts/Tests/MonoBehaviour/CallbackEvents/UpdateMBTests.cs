using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UpdateMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator OnUpdateNotNullAfterStart() {
		var updateMB = new GameObject("obj").AddComponent<UpdateMB>();

		yield return new WaitForEndOfFrame();

		Assert.NotNull(updateMB.onUpdate);
	}

	[UnityTest]
	public IEnumerator OnUpdateNullDefault() {
		var updateMB = new GameObject("obj").AddComponent<UpdateMB>();

		Assert.Null(updateMB.onUpdate);

		yield break;
	}

	[UnityTest]
	public IEnumerator OnUpdateCalled() {
		var called = 0;
		var updateMB = new GameObject("obj").AddComponent<UpdateMB>();

		yield return new WaitForEndOfFrame();

		updateMB.onUpdate.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}
}
