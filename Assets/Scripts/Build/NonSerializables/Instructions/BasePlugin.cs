using System;
using UnityEngine;

public struct PluginHooks
{
	public Action? onBegin;
	public Action? onBeforeYield;
	public Action? onAfterYield;
	public Action? onEnd;

	public static PluginHooks Concat(
		PluginHooks fst,
		PluginHooks snd
	) {
		return new PluginHooks {
			onBegin = fst.onBegin + snd.onBegin,
			onBeforeYield = fst.onBeforeYield + snd.onBeforeYield,
			onAfterYield = fst.onAfterYield + snd.onAfterYield,
			onEnd = fst.onEnd + snd.onEnd,
		};
	}
}

public abstract class BasePlugin<TAgent, TPluginData> : IPlugin
{
	public abstract TAgent GetConcreteAgent(GameObject agent);
	public abstract TPluginData GetPluginData(PluginData data);

	protected abstract PluginHooks GetCallbacks(
		TAgent agent,
		TPluginData data
	);

	public PluginHooksFn PluginHooksFor(GameObject agent) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		return data => {
			TPluginData pluginData = this.GetPluginData(data);
			return this.GetCallbacks(concreteAgent, pluginData);
		};
	}
}
