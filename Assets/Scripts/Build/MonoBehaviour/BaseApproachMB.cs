using UnityEngine;
using System.Collections.Generic;

public abstract class BaseApproachMB<TAppraoch> :
	MonoBehaviour
	where TAppraoch :
		IApproach<Vector3>,
		new()
{
	public Reference agent;
	public float deltaPerSecond;
	public TAppraoch appraoch = new TAppraoch();

	public void Apply(Vector3 position) {
		IEnumerator<WaitForFixedUpdate> routine = this.appraoch.Apply(
			this.agent.GameObject.transform,
			position,
			this.deltaPerSecond
		);
		this.StopAllCoroutines();
		this.StartCoroutine(routine);
	}
}
