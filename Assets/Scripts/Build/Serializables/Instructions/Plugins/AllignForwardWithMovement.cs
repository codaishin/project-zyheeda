using System;
using UnityEngine;

[Serializable]
public class AllignForwardWithMovement : BasePlugin<Transform, PluginData>
{
	public override Transform GetConcreteAgent(GameObject agent) {
		return agent.transform;
	}

	public override PluginData GetPluginData(PluginData data) {
		return data;
	}

	protected override PluginHooks GetCallbacks(
		Transform agent,
		PluginData data
	) {
		Vector3 lastPosition = agent.transform.position;
		Action trackPosition = () => lastPosition = agent.position;
		Action setDirection = () => {
			if (agent.position == lastPosition) {
				return;
			}
			agent.forward = agent.position - lastPosition;
		};
		return new PluginHooks { onAfterYield = setDirection + trackPosition };
	}
}
