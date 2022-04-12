using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BasePluginTests : TestCollection
{
	class MockPluginData : PluginData { }

	class MockMB : MonoBehaviour { }

	class MockPluginSO : BasePlugin<MockMB, MockPluginData>
	{
		public Func<GameObject, MockMB> getConcreteAgent =
			obj => obj.RequireComponent<MockMB>();
		public Func<PluginData, MockPluginData> getPLuginData =
			data => data.As<MockPluginData>()!;
		public Func<MockMB, MockPluginData, PluginHooks> getCallbacks =
			(_, __) => new PluginHooks();

		public override MockMB GetConcreteAgent(GameObject agent) =>
			this.getConcreteAgent(agent);
		public override MockPluginData GetPluginData(PluginData data) =>
			this.getPLuginData(data);
		protected override PluginHooks GetCallbacks(
			MockMB agent,
			MockPluginData data
		) => this.getCallbacks(agent, data);
	}


	[Test]
	public void PluginCallbacksConcat() {
		var called = "";
		var a = new PluginHooks() {
			onBegin = () => called += "a",
			onUpdate = () => called += "a",
			onEnd = () => called += "a",
		};
		var b = new PluginHooks() {
			onBegin = () => called += "b",
			onUpdate = () => called += "b",
			onEnd = () => called += "b",
		};
		var c = PluginHooks.Concat(a, b);

		c.onBegin?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onUpdate?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onEnd?.Invoke();

		Assert.AreEqual("ab", called);
	}

	[UnityTest]
	public IEnumerator GetConcreteAgent() {
		var calledAgent = null as GameObject;
		var calledMB = null as MonoBehaviour;
		var plugin = new MockPluginSO();
		var mockMB = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockMB>();

		plugin.getConcreteAgent = agent => {
			calledAgent = agent;
			return mockMB;
		};
		plugin.getCallbacks = (agent, _) => {
			calledMB = agent;
			return new PluginHooks();
		};

		yield return new WaitForEndOfFrame();

		var partial = plugin.PluginHooksFor(agent.gameObject);

		Assert.AreSame(agent.gameObject, calledAgent);

		partial(new MockPluginData());

		Assert.AreSame(mockMB, calledMB);
	}

	[UnityTest]
	public IEnumerator GetPluginData() {
		var calledPluginData = null as PluginData;
		var calledMockPluginData = null as MockPluginData;
		var plugin = new MockPluginSO();
		var pluginData = new PluginData();
		var mockPluginData = new MockPluginData();
		var agent = new GameObject().AddComponent<MockMB>();


		plugin.getPLuginData = data => {
			calledPluginData = data;
			return mockPluginData;
		};
		plugin.getCallbacks = (_, data) => {
			calledMockPluginData = data;
			return new PluginHooks();
		};

		yield return new WaitForEndOfFrame();

		_ = plugin.PluginHooksFor(agent.gameObject)(pluginData);

		Assert.AreSame(pluginData, calledPluginData);
		Assert.AreSame(mockPluginData, calledMockPluginData);
	}

	[UnityTest]
	public IEnumerator ReturnPluginCallbacks() {
		var called = 0;
		var plugin = new MockPluginSO();
		var agent = new GameObject().AddComponent<MockMB>();
		var callbacks = new PluginHooks { onBegin = () => ++called };

		plugin.getCallbacks = (_, __) => callbacks;

		yield return new WaitForEndOfFrame();

		callbacks = plugin.PluginHooksFor(agent.gameObject)(new PluginData());

		callbacks.onBegin!();

		Assert.AreEqual(1, called);
	}
}
