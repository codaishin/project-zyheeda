using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BasePluginSOTests : TestCollection
{
	class MockPlugin : IPlugin
	{
		public Func<GameObject, PartialPluginCallbacks> getCallbacks =
			_ => _ => new PluginCallbacks();

		public PartialPluginCallbacks GetCallbacks(GameObject agent) =>
			this.getCallbacks(agent);
	}

	class MockPluginSO : BasePluginSO<MockPlugin>
	{
		public MockPlugin Plugin => this.plugin;
	}

	[UnityTest]
	public IEnumerator UseWrappedCallback() {
		var pluginSO = ScriptableObject.CreateInstance<MockPluginSO>();
		var agent = new GameObject();
		var calledAgent = null as GameObject;

		PartialPluginCallbacks callbackFunc = _ => new PluginCallbacks();
		PartialPluginCallbacks getCallbacks(GameObject agent) {
			calledAgent = agent;
			return callbackFunc;
		};

		pluginSO.Plugin.getCallbacks = getCallbacks;

		yield return new WaitForEndOfFrame();

		var calledCallbackFunc = pluginSO.GetCallbacks(agent);

		Assert.AreSame(callbackFunc, calledCallbackFunc);
		Assert.AreSame(agent, calledAgent);
	}
}
