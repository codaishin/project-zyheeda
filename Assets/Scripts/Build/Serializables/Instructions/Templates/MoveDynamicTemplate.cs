using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveDynamicTemplate : BaseInstructionsTemplate<Transform>
{
	[Serializable]
	public struct ValueSet
	{
		public float distance;
		public float speed;
		public float weight;
	}

	public BaseHitSO? hitter;
	public ValueSet min;
	public ValueSet max;

	protected override Transform GetConcreteAgent(GameObject agent) {
		return agent.transform;
	}

	protected override PartialInstructionFunc PartialInstructions(
		Transform agent
	) {
		return data => this.Move(agent, data);
	}

	private IEnumerable<YieldInstruction> Move(Transform agent, PluginData data) {
		Vector3 target = agent.transform.position;
		do {
			yield return new WaitForEndOfFrame();
			target = this.hitter!.TryPoint(agent) ?? target;
			this.MoveFrame(agent, data, target);
		} while (agent.position != target);
	}

	private void MoveFrame(Transform agent, PluginData data, Vector3 target) {
		float lerp = this.GetLerp(agent.position, target);
		agent.position = this.MoveTowards(agent.position, target, lerp);
		data.As<CorePluginData>()!.weight = this.GetWeight(lerp);
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
