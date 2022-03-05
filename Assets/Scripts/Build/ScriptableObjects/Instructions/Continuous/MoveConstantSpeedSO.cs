using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Instructions/MoveConstantSpeed")]
public class MoveConstantSpeedSO : BaseInstructionsSO
{
	public BaseHitSO? hitter;
	public float speed = 1;

	public override CoroutineInstructions InstructionsFor(GameObject agent) {
		Transform transform = agent.transform;
		return () => this.Move(transform);
	}

	private IEnumerable<WaitForFixedUpdate> Move(Transform transform) {
		Vector3? point = this.hitter!.TryPoint(transform);
		if (!point.HasValue) {
			yield break;
		}
		transform.LookAt(point.Value);
		while (transform.position != point.Value) {
			yield return new WaitForFixedUpdate();
			transform.position = this.MoveTowards(transform.position, point.Value);
		}
	}

	private Vector3 MoveTowards(Vector3 current, Vector3 target) {
		return Vector3.MoveTowards(current, target, Time.deltaTime * this.speed);
	}
}
