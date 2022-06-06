using System;
using System.Collections;
using NUnit.Framework;
using Routines;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseRoutinePluginSOTests : TestCollection
{
	class MockPlugin : IPlugin
	{
		public Func<GameObject, PluginFn> getModifierFn =
			_ => _ => null;

		public PluginFn GetPluginFnFor(GameObject agent) =>
			this.getModifierFn(agent);
	}

	class MockPluginSO : BaseRoutinePluginSO<MockPlugin>
	{
		public MockPlugin Modifier => this.plugin;
	}

	[UnityTest]
	public IEnumerator UseWrappedCallback() {
		var pluginSO = ScriptableObject.CreateInstance<MockPluginSO>();
		var agent = new GameObject();
		var calledAgent = null as GameObject;

		PluginFn modifierFn = _ => null as Action;
		PluginFn getModifierFn(GameObject agent) {
			calledAgent = agent;
			return modifierFn;
		};

		pluginSO.Modifier.getModifierFn = getModifierFn;

		yield return new WaitForEndOfFrame();

		var calledModifierFn = pluginSO.GetPluginFnFor(agent);

		Assert.AreSame(modifierFn, calledModifierFn);
		Assert.AreSame(agent, calledAgent);
	}
}
