using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMoveToMB : MonoBehaviour
{
	public float deltaPerSecond;

	public void MoveTo(Vector3 position)
	{
		this.transform.position = this.deltaPerSecond == 0
			? position
			: Vector3.MoveTowards(
				this.transform.position,
				position,
				Time.deltaTime * this.deltaPerSecond
			);
	}
}
