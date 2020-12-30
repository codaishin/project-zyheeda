using UnityEngine;

public class TransformMoveToMB : MonoBehaviour
{
	private Transform agentTransform;

	public Reference agent;
	public float deltaPerSecond;

	private void Start()
	{
		this.agentTransform = this.agent.GameObject.transform;
	}

	private Vector3 Interpolate(in Vector3 position, in float updateDelta)
	{
		return Vector3.MoveTowards(
			this.agentTransform.position,
			position,
			updateDelta * this.deltaPerSecond
		);
	}

	public void MoveTo(Vector3 position)
	{
		this.agentTransform.position = this.deltaPerSecond == 0
			? position
			: this.Interpolate(position, Time.deltaTime);
	}

	public void MoveToFixed(Vector3 position)
	{
		this.agentTransform.position = this.deltaPerSecond == 0
			? position
			: this.Interpolate(position, Time.fixedDeltaTime);
	}
}
