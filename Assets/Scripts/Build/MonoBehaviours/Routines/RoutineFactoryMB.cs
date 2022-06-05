using System;
using Routines;
using UnityEngine;

public class RoutineFactoryMB : MonoBehaviour, IFactory
{
	private Func<IRoutine?>? instructionsFunc;

	public Reference<ITemplate> template;
	public Reference agent;

	private void Start() {
		GameObject agent = this.agent.GameObject;
		this.instructionsFunc = this.template.Value!.GetRoutineFnFor(agent);
	}

	public IRoutine? GetRoutine() {
		return this.instructionsFunc!.Invoke();
	}
}
