using System;
using UnityEngine;

[Serializable]
public class StateAnimation : BasePlugin<IAnimationStates, PluginData>
{
	public Animation.State beginState;
	public Animation.State endState;

	public override IAnimationStates GetConcreteAgent(GameObject agent) {
		return agent.RequireComponent<IAnimationStates>(true);
	}

	public override PluginData GetPluginData(PluginData data) {
		return data;
	}

	protected override PluginHooks GetCallbacks(
		IAnimationStates agent,
		PluginData data
	) {
		return new PluginHooks {
			onBegin = () => agent.Set(this.beginState),
			onEnd = () => agent.Set(this.endState),
		};
	}
}
