using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ValueChannelListenerMBTests : TestCollection
{
	private class MockListenerMB : ValueChannelListenerMB<int> { }

	private class MockChannelSO : ValueChannelSO<int> { }


	[UnityTest]
	public IEnumerator Listen() {
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
	public IEnumerator NoRaiseWhenGOInactive() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<MocklListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();

		listenerMB.listenTo = channelSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		listenerMB.gameObject.SetActive(false);

		yield return new WaitForEndOfFrame();

		channelSO.Raise(101010);

		Assert.AreEqual(0, called);
	}


	[UnityTest]
	public IEnumerator NoRaiseWhenDisabled() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<MockListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();

		listenerMB.listenTo = channelSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		channelSO.Raise(-423);

		Assert.AreEqual(0, called);
	}


	[UnityTest]
	public IEnumerator RemoveListenerOnDestroy() {
		var listenerMB = new GameObject("obj").AddComponent<MocklListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();

		listenerMB.listenTo = channelSO;

		yield return new WaitForEndOfFrame();

		Object.Destroy(listenerMB.gameObject);

		yield return new WaitForEndOfFrame();

		Assert.DoesNotThrow(() => channelSO.Raise(22));
	}


	[UnityTest]
	public IEnumerator ListenAfterDisableAndReEnable() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<MocklListenerMB>();
		var channelSO = ScriptableObject.CreateInstance<MockChannelSO>();

		listenerMB.listenTo = channelSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(v => called = v);

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = true;

		yield return new WaitForEndOfFrame();

		channelSO.Raise(654);

		Assert.AreEqual(654, called);
	}
}
