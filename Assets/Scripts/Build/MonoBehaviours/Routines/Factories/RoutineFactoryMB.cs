using System;
using Routines;
using UnityEngine;

public class RoutineFactoryMB : MonoBehaviour, IFactory
{
	private Func<IRoutine?>? instructionsFunc;

	public Reference<IFuncFactory> routineFuncFactory;
	public Reference agent;

	private void Start() {
		GameObject agent = this.agent.GameObject;
		this.instructionsFunc = this
			.routineFuncFactory
			.Value!
			.GetRoutineFnFor(agent);
	}

	public IRoutine? GetRoutine() {
		return this.instructionsFunc!.Invoke();
	}
}
