using System;
using System.Collections.Generic;
using Routines;
using UnityEngine;

public class RoutineRunMB : MonoBehaviour, IApplicable<IFactory>
{
	public bool delayApply = false;

	private IFactory? currentSource;
	private (Coroutine coroutine, Action nextSubRoutine)? currentlyRunning;

	private void ForceStopCurrent() {
		if (this.currentlyRunning == null) {
			return;
		}
		var (coroutine, _) = this.currentlyRunning.Value;
		this.StopCoroutine(coroutine);
	}

	private Action<IFactory> Delayed(Action<IFactory> action) {
		IEnumerator<YieldInstruction> delay(IFactory source) {
			yield return new WaitForEndOfFrame();
			action(source);
		}
		return source => this.StartCoroutine(delay(source));
	}

	private void NowOrDelayed(Action<IFactory> action, IFactory source) {
		action = this.delayApply
			? this.Delayed(action)
			: action;
		action(source);
	}

	private void ApplyNew(IFactory source) {
		var routine = source.GetRoutine();

		if (routine == null) {
			return;
		}

		this.ForceStopCurrent();
		this.currentSource = source;
		this.currentlyRunning = (
			this.StartCoroutine(routine.GetEnumerator()),
			routine.NextSubRoutine
		);
	}

	private void NextSubRoutineIfCurrentlyRunning(IFactory source) {
		if (this.currentSource != source || this.currentlyRunning == null) {
			return;
		}

		this.currentlyRunning.Value.nextSubRoutine();
	}

	public void Apply(IFactory source) {
		this.NowOrDelayed(this.ApplyNew, source);
	}

	public void Release(IFactory source) {
		this.NowOrDelayed(this.NextSubRoutineIfCurrentlyRunning, source);
	}
}
