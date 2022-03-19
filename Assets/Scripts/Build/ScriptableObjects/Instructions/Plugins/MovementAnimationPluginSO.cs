using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO : BaseInstructionsPluginSO
{
	public override PluginCallbacks GetCallbacks(GameObject agent) {
		IMovementAnimation animation = agent.RequireComponent<IMovementAnimation>();
		return new PluginCallbacks {
			onBegin = data => animation.Move(data.weight),
			onAfterYield = data => animation.Move(data.weight),
			onEnd = _ => animation.Stop(),
		};
	}
}
