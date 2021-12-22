using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ValueChannelListenerMBTests : TestCollection
{
	private class MockListenerMB : ValueChannelListenerMB<int> { }

	private class MockChannelSO : ValueChannelSO<int> { }

	[UnityTest]
	public IEnumerator StartListening() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();

		listenerMB.listenTo = channelSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		channelSO.Raise(33);

		Assert.AreEqual(33, called);
	}

	[UnityTest]
	public IEnumerator StopListening() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();

		listenerMB.listenTo = channelSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		channelSO.Raise(33);

		Assert.AreEqual(0, called);
	}
}
