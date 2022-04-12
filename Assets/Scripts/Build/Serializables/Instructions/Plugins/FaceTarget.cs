using System;
using UnityEngine;

[Serializable]
public class FaceTarget : BasePlugin<Transform, TargetPluginData>
{
	public override Transform GetConcreteAgent(GameObject agent) {
		return agent.transform;
	}

	public override TargetPluginData GetPluginData(PluginData data) {
		return data.Extent<TargetPluginData>();
	}

	protected override PluginHooks GetCallbacks(
		Transform agent,
		TargetPluginData data
	) {
		return new PluginHooks {
			onBegin = FaceTarget.LookAtTarget(agent, data.target),
		};
	}

	private static Action LookAtTarget(Transform agent, Transform? target) {
		return () => {
			if (target == null) {
				return;
			}
			Vector3 position = new Vector3(
				target.position.x,
				agent.position.y,
				target.position.z
			);
			agent.transform.LookAt(position);
		};
	}
}
