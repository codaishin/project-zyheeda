using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsOngoingMB : MonoBehaviour
{
	private Transform agentTransform;

	public Reference agent;

	private void Start() => this.agentTransform = this.agent.GameObject.transform;

	public void BeginMoveTo(Vector3 position)
	{
		this.agentTransform.position = position;
	}
}
