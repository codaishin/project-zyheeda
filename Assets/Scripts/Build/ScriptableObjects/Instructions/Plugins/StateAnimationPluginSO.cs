using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/StateAnimation"
)]
public class StateAnimationPluginSO : BaseInstructionsPluginSO<IAnimationStates, PluginData>
{
	public Animation.State beginState;
	public Animation.State endState;

	public override IAnimationStates GetConcreteAgent(GameObject agent) {
		return agent.RequireComponent<IAnimationStates>(true);
	}

	public override PluginData GetPluginData(PluginData data) {
		return data;
	}

	protected override PluginCallbacks GetCallbacks(
		IAnimationStates agent,
		PluginData data
	) {
		return new PluginCallbacks {
			onBegin = () => agent.Set(this.beginState),
			onEnd = () => agent.Set(this.endState),
		};
	}
}
