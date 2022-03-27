using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO :
	BaseInstructionsPluginSO<IAnimationStates, CorePluginData>
{
	public override IAnimationStates GetConcreteAgent(GameObject agent) {
		return agent.RequireComponent<IAnimationStates>(true);
	}

	public override CorePluginData GetPluginData(PluginData data) {
		return data.As<CorePluginData>()!;
	}

	protected override PluginCallbacks GetCallbacks(
		IAnimationStates agent,
		CorePluginData data
	) {
		return new PluginCallbacks {
			onBegin = () => {
				agent.Set(Animation.State.WalkOrRun);
				agent.Blend(Animation.BlendState.WalkOrRun, data.weight);
			},
			onAfterYield = () => {
				agent.Blend(Animation.BlendState.WalkOrRun, data.weight);
			},
			onEnd = () => {
				agent.Set(Animation.State.Idle);
			},
		};
	}
}
