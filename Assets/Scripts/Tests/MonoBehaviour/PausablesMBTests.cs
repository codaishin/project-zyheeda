using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PausablesMBTests : TestCollection
{
	private class MockPausableMB : MonoBehaviour, IPausable
	{
		public bool Paused { get; set; }
	}

	[UnityTest]
	public IEnumerator PauseChildren()
	{
		var parent = new GameObject("pausables").AddComponent<PausablesMB>();
		var childA = new GameObject("pausableA").AddComponent<MockPausableMB>();
		var childB = new GameObject("pausableB").AddComponent<MockPausableMB>();

		childA.transform.parent = parent.transform;
		childB.transform.parent = parent.transform;

		yield return new WaitForEndOfFrame();

		parent.PauseChildren(true);

		Assert.AreEqual((true, true), (childA.Paused, childB.Paused));
	}
}
