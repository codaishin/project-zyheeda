using System;
using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO : BaseInstructionsPluginSO
{
	public override Func<PluginData, PluginCallbacks> GetCallbacks(
		GameObject agent
	) {
		IAnimationStates animation = agent.RequireComponent<IAnimationStates>(true);
		return data => {
			CorePluginData weightData = data.As<CorePluginData>()!;
			return new PluginCallbacks {
				onBegin = () => {
					animation.Set(Animation.State.WalkOrRun);
					animation.Blend(
						Animation.BlendState.WalkOrRun,
						weightData.weight
					);
				},
				onAfterYield = () => {
					animation.Blend(
						Animation.BlendState.WalkOrRun,
						weightData.weight
					);
				},
				onEnd = () => {
					animation.Set(Animation.State.Idle);
				},
			};
		};
	}
}
