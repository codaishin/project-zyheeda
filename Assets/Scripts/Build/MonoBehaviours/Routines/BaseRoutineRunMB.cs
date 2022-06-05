using System;
using System.Collections.Generic;
using System.Linq;
using Routines;
using UnityEngine;

public abstract class BaseRoutineRunMB<TKey> :
	MonoBehaviour,
	IApplicable<(TKey, Func<IRoutine?>)>,
	IStoppable<TKey>
{
	private (TKey key, IRoutine routine, Coroutine coroutine)? current;


	private static IEnumerable<bool> Exhaust(IRoutine routine) {
		while (routine.NextSubRoutine()) {
			yield return false;
		};
		yield return true;
	}

	private void ForceStopCurrentCoroutine() {
		if (this.current == null) {
			return;
		}
		this.StopCoroutine(this.current.Value.coroutine);
		this.current = null;
	}

	private IRoutine? GetRunning(TKey key) {
		if (this.current is null || this.current.Value.key is null) {
			return null;
		}
		if (this.current.Value.key.Equals(key) == false) {
			return null;
		}
		return this.current.Value.routine;
	}

	public void Apply((TKey, Func<IRoutine?>) value) {
		var (key, getRoutine) = value;

		var routine = this.GetRunning(key);
		if (routine is not null && routine.NextSubRoutine()) {
			return;
		}

		routine = getRoutine();

		if (routine is null) {
			return;
		}

		this.ForceStopCurrentCoroutine();
		var coroutine = this.StartCoroutine(routine.GetEnumerator());
		this.current = (key, routine, coroutine);
	}

	public void Stop(TKey key, int softStopAttempts) {
		if (this.current is null) {
			return;
		}

		var currentKey = this.current.Value.key;

		if (currentKey is null || currentKey.Equals(key) == false) {
			return;
		}

		var currentRoutine = this.current.Value.routine;
		var exhaustedSubRoutines = BaseRoutineRunMB<TKey>
			.Exhaust(currentRoutine)
			.Take(softStopAttempts)
			.Last();

		if (exhaustedSubRoutines) {
			return;
		}

		this.ForceStopCurrentCoroutine();
	}
}
