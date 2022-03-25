using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO : BaseInstructionsPluginSO
{
	public override PluginCallbacks GetCallbacks(GameObject agent) {
		IAnimationStates animation = agent.RequireComponent<IAnimationStates>(true);
		return new PluginCallbacks {
			onBegin = data => {
				animation.Set(Animation.State.WalkOrRun);
				animation.Blend(Animation.BlendState.WalkOrRun, data.weight);
			},
			onAfterYield = data => {
				animation.Blend(Animation.BlendState.WalkOrRun, data.weight);
			},
			onEnd = _ => {
				animation.Set(Animation.State.Idle);
			},
		};
	}
}
