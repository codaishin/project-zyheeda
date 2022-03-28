using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveConstant : BaseInstructions<Transform>
{
	public BaseHitSO? hitter;
	public float speed = 1;
	public float weight = 1;

	protected override Transform GetConcreteAgent(GameObject agent) {
		return agent.transform;
	}

	protected override PartialInstructionFunc PartialInstructions(
		Transform agent
	) {
		return data => this.Move(agent, data);
	}

	private IEnumerable<WaitForEndOfFrame> Move(
		Transform transform,
		PluginData data
	) {
		Vector3? point = this.hitter!.TryPoint(transform);

		data.As<CorePluginData>()!.weight = this.weight;
		if (!point.HasValue) {
			yield break;
		}
		while (transform.position != point.Value) {
			yield return new WaitForEndOfFrame();
			transform.position = this.MoveTowards(transform.position, point.Value);
		}
	}

	private Vector3 MoveTowards(Vector3 current, Vector3 target) {
		return Vector3.MoveTowards(current, target, Time.deltaTime * this.speed);
	}
}
