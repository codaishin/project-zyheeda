using System;
using UnityEngine;

public struct PluginCallbacks
{
	public Action? onBegin;
	public Action? onBeforeYield;
	public Action? onAfterYield;
	public Action? onEnd;

	public static PluginCallbacks operator +(
		PluginCallbacks fst,
		PluginCallbacks snd
	) {
		return new PluginCallbacks {
			onBegin = fst.onBegin + snd.onBegin,
			onBeforeYield = fst.onBeforeYield + snd.onBeforeYield,
			onAfterYield = fst.onAfterYield + snd.onAfterYield,
			onEnd = fst.onEnd + snd.onEnd,
		};
	}
}

public abstract class BaseInstructionsPluginSO : ScriptableObject
{
	public abstract PartialPluginCallbacks GetCallbacks(GameObject agent);
}

public abstract class BaseInstructionsPluginSO<TAgent, TPluginData> :
	BaseInstructionsPluginSO
{
	public abstract TAgent GetConcreteAgent(GameObject agent);
	public abstract TPluginData GetPluginData(PluginData data);

	protected abstract PluginCallbacks GetCallbacks(
		TAgent agent,
		TPluginData data
	);

	public override PartialPluginCallbacks GetCallbacks(
		GameObject agent
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		return data => {
			TPluginData pluginData = this.GetPluginData(data);
			return this.GetCallbacks(concreteAgent, pluginData);
		};
	}
}
