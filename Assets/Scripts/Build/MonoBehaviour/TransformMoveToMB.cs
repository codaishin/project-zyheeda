using UnityEngine;

public class TransformMoveToMB : MonoBehaviour
{
	public float deltaPerSecond;

	private Vector3 Interpolate(in Vector3 position, in float updateDelta)
	{
		return Vector3.MoveTowards(
			this.transform.position,
			position,
			updateDelta * this.deltaPerSecond
		);
	}

	public void MoveTo(Vector3 position)
	{
		this.transform.position = this.deltaPerSecond == 0
			? position
			: this.Interpolate(position, Time.deltaTime);
	}

	public void MoveToFixed(Vector3 position)
	{
		this.transform.position = this.deltaPerSecond == 0
			? position
			: this.Interpolate(position, Time.fixedDeltaTime);
	}
}
