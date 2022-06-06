using System;
using Routines;
using UnityEngine;

public abstract class BaseRoutineTemplateMB<TFactory> :
	MonoBehaviour,
	ITemplate
	where TFactory :
		ITemplate,
		new()
{
	[SerializeField]
	private TFactory template = new TFactory();

	public TFactory Template => this.template;

	public Func<IRoutine?> GetRoutineFnFor(GameObject agent) {
		return this.template.GetRoutineFnFor(agent);
	}
}
