using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseApproachMB<TAppraoch, TTarget> :
	MonoBehaviour
	where TAppraoch :
		IApproach<TTarget>,
		new()
{
	private MonoBehaviour? runner;

	public Reference agent;
	public float deltaPerSecond;
	public TAppraoch appraoch = new TAppraoch();
	public CoroutineRunnerMB? externalRunner;
	public UnityEvent onEnd = new UnityEvent();

	private void Start() {
		this.runner = ((MonoBehaviour?)this.externalRunner) ?? this;
	}

	public void Begin(TTarget position) {
		this.runner!.StopAllCoroutines();
		this.runner!.StartCoroutine(this.GetRoutine(position));
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
