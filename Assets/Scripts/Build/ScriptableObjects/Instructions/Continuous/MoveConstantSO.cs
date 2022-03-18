using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Instructions/MoveConstantSpeed")]
public class MoveConstantSO : BaseInstructionsSO<Transform>
{
	public BaseHitSO? hitter;
	public float speed = 1;
	public float weight = 1;

	protected override Transform GetConcreteAgent(GameObject agent) {
		return agent.transform;
	}

	protected override CoroutineInstructions Instructions(
		Transform agent,
		PluginData data
	) {
		data.weight = this.weight;
		return () => this.Move(agent);
	}

	private IEnumerable<WaitForEndOfFrame> Move(Transform transform) {
		Vector3? point = this.hitter!.TryPoint(transform);
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
