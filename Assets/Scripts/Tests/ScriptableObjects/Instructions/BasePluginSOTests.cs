using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BasePluginSOTests : TestCollection
{
	class MockPlugin : IPlugin
	{
		public Func<GameObject, PluginHooksFn> getCallbacks =
			_ => _ => new PluginHooks();

		public PluginHooksFn PluginHooksFor(GameObject agent) =>
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

		PluginHooksFn callbackFunc = _ => new PluginHooks();
		PluginHooksFn getCallbacks(GameObject agent) {
			calledAgent = agent;
			return callbackFunc;
		};

		pluginSO.Plugin.getCallbacks = getCallbacks;

		yield return new WaitForEndOfFrame();

		var calledCallbackFunc = pluginSO.PluginHooksFor(agent);

		Assert.AreSame(callbackFunc, calledCallbackFunc);
		Assert.AreSame(agent, calledAgent);
	}
}
