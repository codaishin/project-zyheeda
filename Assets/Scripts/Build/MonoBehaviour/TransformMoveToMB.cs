using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMoveToMB : MonoBehaviour
{
	public void MoveTo(Vector3 position)
	{
		this.transform.position = position;
	}
}
