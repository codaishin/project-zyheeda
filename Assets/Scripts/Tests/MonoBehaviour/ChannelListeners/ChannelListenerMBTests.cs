using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChannelListenerMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator StartListening() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<ChannelListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo =
			Reference<IChannel>.PointToScriptableObject(channelSO);

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		channelSO.Raise();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator StopListening() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<ChannelListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo =
			Reference<IChannel>.PointToScriptableObject(channelSO);

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		channelSO.Raise();

		Assert.AreEqual(0, called);
	}
}
