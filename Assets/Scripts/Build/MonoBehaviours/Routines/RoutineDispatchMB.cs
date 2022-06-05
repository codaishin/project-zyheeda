using System;
using Routines;
using UnityEngine;

public class RoutineDispatchMB : MonoBehaviour, IApplicable
{
	private Func<IRoutine?>? getRoutine;

	public Reference<ITemplate> template;
	public Reference agent;
	public Reference<IApplicable<(RoutineDispatchMB, Func<IRoutine?>)>> runner;

	private void Start() {
		GameObject agent = this.agent.GameObject;
		this.getRoutine = this.template.Value!.GetRoutineFnFor(agent);
	}

	public void Apply() {
		if (this.getRoutine is null) {
			throw new NullReferenceException("ILLEGAL NULLIFICATION");
		}
		this.runner.Value!.Apply((this, this.getRoutine));
	}
}
