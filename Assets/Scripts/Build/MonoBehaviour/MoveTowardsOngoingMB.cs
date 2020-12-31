using System.Collections;
using UnityEngine;

public class MoveTowardsOngoingMB : MonoBehaviour
{
	private IEnumerator currentMove;
	private Transform agentTransform;

	public Reference agent;
	public float deltaPerSecond;

	private void Start() => this.agentTransform = this.agent.GameObject.transform;

	private Vector3 Interpolate(in Vector3 target) => Vector3.MoveTowards(
		this.agentTransform.position,
		target,
		this.deltaPerSecond * Time.fixedDeltaTime
	);

	public void BeginMoveTo(Vector3 position)
	{
		this.StopMoving();
		this.currentMove = this.MoveTo(position);
		this.StartCoroutine(this.currentMove);
	}

	public void StopMoving()
	{
		if (this.currentMove != null) {
			this.StopCoroutine(this.currentMove);
		}
	}

	private IEnumerator MoveTo(Vector3 position)
	{
		while (this.agentTransform.position != position) {
			yield return new WaitForFixedUpdate();
			this.agentTransform.position = this.deltaPerSecond == default
				? position
				: this.Interpolate(position);
		}
	}
}
