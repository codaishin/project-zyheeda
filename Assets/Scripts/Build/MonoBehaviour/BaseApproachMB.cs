using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseApproachMB<TAppraoch, TTarget> :
	MonoBehaviour
	where TAppraoch :
		IApproach<TTarget>,
		new()
{
	public Reference agent;
	public float deltaPerSecond;
	public TAppraoch appraoch = new TAppraoch();

	public UnityEvent onEnd = new UnityEvent();

	public void Begin(TTarget position) {
		this.StopAllCoroutines();
		this.StartCoroutine(this.GetRoutine(position));
	}

	private IEnumerator<WaitForFixedUpdate> GetRoutine(TTarget position) {
		IEnumerator<WaitForFixedUpdate> routine = this.appraoch.Apply(
			this.agent.GameObject.transform,
			position,
			this.deltaPerSecond
		);
		while (routine.MoveNext()) {
			yield return routine.Current;
		}
		this.onEnd.Invoke();
	}
}
