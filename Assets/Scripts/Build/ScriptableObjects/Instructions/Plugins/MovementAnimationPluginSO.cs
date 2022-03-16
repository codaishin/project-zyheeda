using System;
using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO : BaseInstructionsPluginSO<MoveData>
{
	public float walkOrRunWeight;

	public override Action<MoveData> GetOnBegin(GameObject agent) {
		IMovementAnimation animation = agent.RequireComponent<IMovementAnimation>();
		float weight = this.walkOrRunWeight;
		return _ => animation.Move(weight);
	}

	public override Action<MoveData> GetOnEnd(GameObject agent) {
		IMovementAnimation animation = agent.RequireComponent<IMovementAnimation>();
		return _ => animation.Stop();
	}
}
