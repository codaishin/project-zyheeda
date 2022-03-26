using System;
using UnityEngine;

[CreateAssetMenu(
	menuName = "ScriptableObjects/Instructions/Plugins/AllignForwardWithMovement"
)]
public class AllignForwardWithMovementSO : BaseInstructionsPluginSO
{
	public override PluginCallbacks GetCallbacks(GameObject agent) {
		Transform transform = agent.transform;
		Vector3 lastPosition = agent.transform.position;

		Action<PluginData> trackPosition =
			_ => lastPosition = transform.position;
		Action<PluginData> setDirection =
			_ => {
				if (transform.position == lastPosition) {
					return;
				}
				transform.forward = transform.position - lastPosition;
			};

		return new PluginCallbacks { onAfterYield = setDirection + trackPosition };
	}
}
