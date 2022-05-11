using System;
using System.Collections.Generic;
using System.Linq;
using Routines;
using UnityEngine;

public class RoutineRunMB :
	MonoBehaviour,
	IApplicable<IFactory>,
	IStoppable<IFactory>
{
	public bool delayApply = false;
	private (IFactory source, IRoutine routine, Coroutine coroutine)? current;

	private void ForceStopCurrentCoroutine() {
		if (this.current == null) {
			return;
		}
		this.StopCoroutine(this.current.Value.coroutine);
		this.current = null;
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

	private IRoutine? GetRoutineIfCurrentIs(IFactory source) {
		if (this.current == null) {
			return null;
		}
		if (this.current.Value.source != source) {
			return null;
		}
		return this.current.Value.routine;
	}

	private void StartRoutineOrNextSubRoutine(IFactory source) {
		var currentRoutine = this.GetRoutineIfCurrentIs(source);
		if (currentRoutine is not null && currentRoutine.NextSubRoutine()) {
			return;
		}

		var newRoutine = source.GetRoutine();
		if (newRoutine is null) {
			return;
		}

		this.ForceStopCurrentCoroutine();

		var newCoroutine = this.StartCoroutine(newRoutine.GetEnumerator());

		this.current = (source, newRoutine, newCoroutine);
	}

	public void Apply(IFactory source) {
		this.NowOrDelayed(this.StartRoutineOrNextSubRoutine, source);
	}

	private static IEnumerable<bool> ExhaustSubRoutines(IRoutine routine) {
		while (routine.NextSubRoutine()) {
			yield return false;
		};
		yield return true;
	}

	public void Stop(IFactory value, int softStopAttempts) {
		if (this.current?.source != value) {
			return;
		}

		var routine = this.current.Value.routine;
		var exhaustedSubRoutines = RoutineRunMB
			.ExhaustSubRoutines(routine)
			.Take(softStopAttempts)
			.Last();

		if (exhaustedSubRoutines) {
			return;
		}

		this.ForceStopCurrentCoroutine();
	}
}
