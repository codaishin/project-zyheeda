using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseChannelListenerMBTests : TestCollection
{
	private class MockListenerMB : BaseChannelListenerMB
	{
		private List<string> calls = new List<string>();

		public IEnumerable<string> Calls => calls;

		public override void StartListening() => this.calls.Add("start");
		public override void StopListening() => this.calls.Add("stop");
	}

	[UnityTest]
	public IEnumerator StartListening() {
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();

		yield return new WaitForEndOfFrame();

		Assert.AreEqual("start", listenerMB.Calls.Last());
	}


	[UnityTest]
	public IEnumerator StopListeningWhenInactive() {
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();

		yield return new WaitForEndOfFrame();

		listenerMB.gameObject.SetActive(false);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual("stop", listenerMB.Calls.Last());
	}

	[UnityTest]
	public IEnumerator StopListeningWhenDisabled() {
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual("stop", listenerMB.Calls.Last());
	}

	[UnityTest]
	public IEnumerator StopListeningWhenDestroyed() {
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();

		yield return new WaitForEndOfFrame();

		GameObject.Destroy(listenerMB);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual("stop", listenerMB.Calls.Last());
	}

	[UnityTest]
	public IEnumerator RestartListeningWhenReenabled() {
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = true;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual("start", listenerMB.Calls.Last());
	}
}
