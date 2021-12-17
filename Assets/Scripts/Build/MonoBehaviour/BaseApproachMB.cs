using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public abstract class BaseApproachMB<TAppraoch, TTarget> :
	MonoBehaviour
	where TAppraoch :
		IApproach<TTarget>,
		new()
{
	public Reference agent;
	public float deltaPerSecond;
	public TAppraoch appraoch = new TAppraoch();

	public UnityEvent onBegin = new UnityEvent();
	public UnityEvent onEnd = new UnityEvent();

	public void Apply(TTarget position) {
		this.StopAllCoroutines();
		this.StartCoroutine(this.GetRoutine(position));
	}

	private IEnumerator<WaitForFixedUpdate> GetRoutine(TTarget position) {
		IEnumerator<WaitForFixedUpdate> routine = this.appraoch.Apply(
			this.agent.GameObject.transform,
			position,
			this.deltaPerSecond
		);
		this.onBegin.Invoke();
		while (routine.MoveNext()) {
			yield return routine.Current;
		}
		this.onEnd.Invoke();
	}
}
