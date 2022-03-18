using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO : BaseInstructionsPluginSO
{
	public override PluginCallbacks GetCallbacks(
		GameObject agent,
		PluginData data
	) {
		IMovementAnimation animation = agent.RequireComponent<IMovementAnimation>();
		return new PluginCallbacks {
			onBegin = () => animation.Move(data.weight),
			onUpdate = () => animation.Move(data.weight),
			onEnd = () => animation.Stop(),
		};
	}
}
