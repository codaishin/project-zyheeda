using System.Collections;
using UnityEngine;

public class MoveTowardsOngoingMB : MonoBehaviour, IPausable<WaitForFixedUpdate>
{
	private IEnumerator currentMove;
	private Transform agentTransform;

	public Reference agent;
	public float deltaPerSecond;

	public bool Paused { get; set; }

	public WaitForFixedUpdate Pause => new WaitForFixedUpdate();

	private void Start() => this.agentTransform = this.agent.GameObject.transform;

	private Vector3 Interpolate(in Vector3 target) => Vector3.MoveTowards(
		this.agentTransform.position,
		target,
		this.deltaPerSecond * Time.fixedDeltaTime
	);

	public void BeginMoveTo(Vector3 position)
	{
		this.StopMoving();
		this.currentMove = this.Manage(this.MoveTo(position));
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
			this.agentTransform.position = this.deltaPerSecond == default
				? position
				: this.Interpolate(position);
			yield return new WaitForFixedUpdate();
		}
	}
}
