using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveDynamicTemplate : BaseInstructionsTemplate<MovementData>
{
	[Serializable]
	public struct ValueSet
	{
		public float distance;
		public float speed;
		public float weight;
	}

	public Reference<IHit> hitter;
	public ValueSet min;
	public ValueSet max;

	protected override MovementData ConcreteAgent(GameObject agent) {
		return new MovementData {
			transform = agent.transform,
			getTarget = this.hitter.Value!.TryPoint(agent),
		};
	}

	protected override InternalInstructionFn InternalInstructionsFn(
		MovementData agent
	) {
		return data => this.Move(agent, data);
	}

	private IEnumerable<YieldInstruction> Move(
		MovementData agent,
		PluginData data
	) {
		Vector3 target = agent.transform.position;
		Transform transform = agent.transform;
		CorePluginData corePluginData = data.As<CorePluginData>()!;
		do {
			yield return new WaitForEndOfFrame();
			target = agent.getTarget() ?? target;
			this.MoveFrame(transform, corePluginData, target);
		} while (transform.position != target);
	}

	private void MoveFrame(
		Transform transform,
		CorePluginData corePluginData,
		Vector3 target
	) {
		float lerp = this.GetLerp(transform.position, target);
		transform.position = this.MoveTowards(transform.position, target, lerp);
		corePluginData.weight = this.GetWeight(lerp);
	}

	private Vector3 MoveTowards(Vector3 current, Vector3 target, float lerp) {
		float speed = Mathf.Lerp(this.min.speed, this.max.speed, lerp);
		return Vector3.MoveTowards(current, target, Time.deltaTime * speed);
	}

	private float GetLerp(Vector3 current, Vector3 target) {
		return Mathf.InverseLerp(
			this.min.distance,
			this.max.distance,
			(target - current).magnitude
		);
	}

	private float GetWeight(float lerp) {
		return Mathf.Lerp(this.min.weight, this.max.weight, lerp);
	}
}
