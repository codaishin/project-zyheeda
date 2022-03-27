using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BasePluginInstructionSOTests : TestCollection
{
	[Test]
	public void PluginCallbacksAdd() {
		var called = "";
		var a = new PluginCallbacks() {
			onBegin = () => called += "a",
			onBeforeYield = () => called += "a",
			onAfterYield = () => called += "a",
			onEnd = () => called += "a",
		};
		var b = new PluginCallbacks() {
			onBegin = () => called += "b",
			onBeforeYield = () => called += "b",
			onAfterYield = () => called += "b",
			onEnd = () => called += "b",
		};
		var c = a + b;

		c.onBegin?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onBeforeYield?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onAfterYield?.Invoke();

		Assert.AreEqual("ab", called);

		called = "";
		c.onEnd?.Invoke();

		Assert.AreEqual("ab", called);
	}

	class MockPluginData : PluginData { }
	class MockMB : MonoBehaviour { }

	class MockPluginSO : BaseInstructionsPluginSO<MockMB, MockPluginData>
	{
		public Func<GameObject, MockMB> getConcreteAgent =
			obj => obj.RequireComponent<MockMB>();
		public Func<PluginData, MockPluginData> getPLuginData =
			data => data.As<MockPluginData>()!;
		public Func<MockMB, MockPluginData, PluginCallbacks> getCallbacks =
			(_, __) => new PluginCallbacks();

		public override MockMB GetConcreteAgent(GameObject agent) =>
			this.getConcreteAgent(agent);
		public override MockPluginData GetPluginData(PluginData data) =>
			this.getPLuginData(data);
		protected override PluginCallbacks GetCallbacks(
			MockMB agent,
			MockPluginData data
		) => this.getCallbacks(agent, data);
	}

	[UnityTest]
	public IEnumerator GetConcreteAgent() {
		var calledAgent = null as GameObject;
		var calledMB = null as MonoBehaviour;
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();
		var mockMB = new GameObject().AddComponent<MockMB>();
		var agent = new GameObject().AddComponent<MockMB>();

		plugin.getConcreteAgent = agent => {
			calledAgent = agent;
			return mockMB;
		};
		plugin.getCallbacks = (agent, _) => {
			calledMB = agent;
			return new PluginCallbacks();
		};

		yield return new WaitForEndOfFrame();

		var partial = plugin.GetCallbacks(agent.gameObject);

		Assert.AreSame(agent.gameObject, calledAgent);

		partial(new MockPluginData());

		Assert.AreSame(mockMB, calledMB);
	}

	[UnityTest]
	public IEnumerator GetPluginData() {
		var calledPluginData = null as PluginData;
		var calledMockPluginData = null as MockPluginData;
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();
		var pluginData = new PluginData();
		var mockPluginData = new MockPluginData();
		var agent = new GameObject().AddComponent<MockMB>();


		plugin.getPLuginData = data => {
			calledPluginData = data;
			return mockPluginData;
		};
		plugin.getCallbacks = (_, data) => {
			calledMockPluginData = data;
			return new PluginCallbacks();
		};

		yield return new WaitForEndOfFrame();

		_ = plugin.GetCallbacks(agent.gameObject)(pluginData);

		Assert.AreSame(pluginData, calledPluginData);
		Assert.AreSame(mockPluginData, calledMockPluginData);
	}

	[UnityTest]
	public IEnumerator ReturnPluginCallbacks() {
		var called = 0;
		var plugin = ScriptableObject.CreateInstance<MockPluginSO>();
		var agent = new GameObject().AddComponent<MockMB>();
		var callbacks = new PluginCallbacks { onBegin = () => ++called };

		plugin.getCallbacks = (_, __) => callbacks;

		yield return new WaitForEndOfFrame();

		callbacks = plugin.GetCallbacks(agent.gameObject)(new PluginData());

		callbacks.onBegin!();

		Assert.AreEqual(1, called);
	}
}
