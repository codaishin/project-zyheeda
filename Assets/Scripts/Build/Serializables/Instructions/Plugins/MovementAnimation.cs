using System;
using UnityEngine;

public struct MovementAnimators
{
	public IAnimationStatesBlend statesBlend;
	public IAnimationStates states;
}

[Serializable]
public class MovementAnimation : BasePlugin<MovementAnimators, CorePluginData>
{
	public override MovementAnimators GetConcreteAgent(GameObject agent) {
		return new MovementAnimators {
			statesBlend = agent.RequireComponent<IAnimationStatesBlend>(true),
			states = agent.RequireComponent<IAnimationStates>(true),
		};
	}

	public override CorePluginData GetPluginData(PluginData data) {
		return data.As<CorePluginData>()!;
	}

	protected override PluginCallbacks GetCallbacks(
		MovementAnimators agent,
		CorePluginData data
	) {
		return new PluginCallbacks {
			onBegin = () => {
				agent.states.Set(Animation.State.WalkOrRun);
				agent.statesBlend.Blend(Animation.BlendState.WalkOrRun, data.weight);
			},
			onAfterYield = () => {
				agent.statesBlend.Blend(Animation.BlendState.WalkOrRun, data.weight);
			},
			onEnd = () => {
				agent.states.Set(Animation.State.Idle);
			},
		};
	}
}
