using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChannelListenerMBTests : TestCollection
{
	class MockActionMB : MonoBehaviour, IApplicable
	{
		public int called = 0;
		public void Apply() => ++this.called;

		public void Release() {
			throw new System.NotImplementedException();
		}
	}

	[UnityTest]
	public IEnumerator StartListening() {
		var listenerMB = new GameObject("obj").AddComponent<ChannelListenerMB>();
		var actionMB = new GameObject().AddComponent<MockActionMB>();
		var channelSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(channelSO),
		};
		listenerMB.apply = new Reference<IApplicable>[]{
			Reference<IApplicable>.PointToComponent(actionMB),
		};

		yield return new WaitForEndOfFrame();

		channelSO.Raise();

		Assert.AreEqual(1, actionMB.called);
	}

	[UnityTest]
	public IEnumerator StopListening() {
		var listenerMB = new GameObject("obj").AddComponent<ChannelListenerMB>();
		var actionMB = new GameObject().AddComponent<MockActionMB>();
		var channelSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(channelSO),
		};
		listenerMB.apply = new Reference<IApplicable>[]{
			Reference<IApplicable>.PointToComponent(actionMB),
		};

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		channelSO.Raise();

		Assert.AreEqual(0, actionMB.called);
	}

	[UnityTest]
	public IEnumerator StartListeningMultiple() {
		var listenerMB = new GameObject("obj").AddComponent<ChannelListenerMB>();
		var actionMB = new GameObject().AddComponent<MockActionMB>();
		var channelSOs = new ChannelSO[] {
			ScriptableObject.CreateInstance<ChannelSO>(),
			ScriptableObject.CreateInstance<ChannelSO>(),
		};

		listenerMB.listenTo = channelSOs
			.Select(Reference<IChannel>.PointToScriptableObject)
			.ToArray();
		listenerMB.apply = new Reference<IApplicable>[]{
			Reference<IApplicable>.PointToComponent(actionMB),
		};

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		channelSOs.ForEach(c => c.Raise());

		Assert.AreEqual(2, actionMB.called);
	}

	[UnityTest]
	public IEnumerator StopListeningMultiple() {
		var listenerMB = new GameObject("obj").AddComponent<ChannelListenerMB>();
		var actionMB = new GameObject().AddComponent<MockActionMB>();
		var channelSOs = new ChannelSO[] {
			ScriptableObject.CreateInstance<ChannelSO>(),
			ScriptableObject.CreateInstance<ChannelSO>(),
		};

		listenerMB.listenTo = channelSOs
			.Select(Reference<IChannel>.PointToScriptableObject)
			.ToArray();
		listenerMB.apply = new Reference<IApplicable>[]{
			Reference<IApplicable>.PointToComponent(actionMB),
		};

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		listenerMB.enabled = false;

		yield return new WaitForEndOfFrame();

		channelSOs.ForEach(c => c.Raise());

		Assert.AreEqual(0, actionMB.called);
	}

	[UnityTest]
	public IEnumerator StartListeningMultipleApplies() {
		var listenerMB = new GameObject("obj").AddComponent<ChannelListenerMB>();
		var actionMBs = new MockActionMB[] {
			 new GameObject().AddComponent<MockActionMB>(),
			 new GameObject().AddComponent<MockActionMB>(),
		};
		var channelSO = ScriptableObject.CreateInstance<ChannelSO>();

		listenerMB.listenTo = new Reference<IChannel>[] {
			Reference<IChannel>.PointToScriptableObject(channelSO),
		};
		listenerMB.apply = actionMBs
			.Select(Reference<IApplicable>.PointToComponent)
			.ToArray();

		yield return new WaitForEndOfFrame();

		channelSO.Raise();

		CollectionAssert.AreEqual(
			new int[] { 1, 1 },
			actionMBs.Select(a => a.called)
		);
	}
}
