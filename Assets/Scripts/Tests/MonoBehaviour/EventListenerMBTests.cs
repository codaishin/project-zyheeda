using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EventListenerMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator OnRaiseNotNullAfterStart() {
		var listenerMB = new GameObject("obj").AddComponent<EventListenerMB>();
		var eventSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = eventSO;

		yield return new WaitForEndOfFrame();

		Assert.NotNull(listenerMB.onRaise);
	}

	[UnityTest]
	public IEnumerator OnRaiseDefaultNull() {
		var listenerMB = new GameObject("obj").AddComponent<EventListenerMB>();

		Assert.Null(listenerMB.onRaise);

		yield break;
	}

	[UnityTest]
	public IEnumerator Listen() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<EventListenerMB>();
		var eventSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = eventSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		eventSO.Raise();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator NoRaiseWhenGOInactive() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<EventListenerMB>();
		var eventSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = eventSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		listenerMB.gameObject.SetActive(false);

		yield return new WaitForEndOfFrame();

		eventSO.Raise();

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator NoRaiseWhenDisabled() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<EventListenerMB>();
		var eventSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = eventSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		eventSO.Raise();

		Assert.AreEqual(0, called);
	}

	[UnityTest]
	public IEnumerator RemoveListenerOnDestroy() {
		var listenerMB = new GameObject("obj").AddComponent<EventListenerMB>();
		var eventSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = eventSO;

		yield return new WaitForEndOfFrame();

		Object.Destroy(listenerMB.gameObject);

		yield return new WaitForEndOfFrame();

		Assert.DoesNotThrow(() => eventSO.Raise());
	}

	[UnityTest]
	public IEnumerator ListenAfterDisableAndReEnable() {
		var called = 0;
		var listenerMB = new GameObject("obj").AddComponent<EventListenerMB>();
		var eventSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = eventSO;

		yield return new WaitForEndOfFrame();

		listenerMB.onRaise!.AddListener(() => ++called);

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		listenerMB.enabled = true;

		yield return new WaitForEndOfFrame();

		eventSO.Raise();

		Assert.AreEqual(1, called);
	}
}
