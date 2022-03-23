using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO : BaseInstructionsPluginSO
{
	public override PluginCallbacks GetCallbacks(GameObject agent) {
		IMovementAnimation moveAnimation =
			agent.RequireComponent<IMovementAnimation>();
		return new PluginCallbacks {
			onBegin = data => {
				moveAnimation.Begin();
				moveAnimation.WalkOrRunBlend(data.weight);
			},
			onAfterYield = data => {
				moveAnimation.WalkOrRunBlend(data.weight);
			},
			onEnd = _ => {
				moveAnimation.End();
			},
		};
	}
}
