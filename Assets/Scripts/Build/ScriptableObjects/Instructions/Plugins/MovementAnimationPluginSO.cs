using System;
using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/MovementAnimation"
)]
public class MovementAnimationPluginSO : BaseInstructionsPluginSO
{
	public override Action GetOnBegin(GameObject agent, PluginData data) {
		IMovementAnimation animation = agent.RequireComponent<IMovementAnimation>();
		return () => animation.Move(data.weight);
	}

	public override Action? GetOnUpdate(GameObject agent, PluginData data) {
		return null;
	}

	public override Action GetOnEnd(GameObject agent, PluginData data) {
		IMovementAnimation animation = agent.RequireComponent<IMovementAnimation>();
		return () => animation.Stop();
	}
}
