using System;
using Routines;
using UnityEngine;

public abstract class BaseRoutineTemplateSO<TRoutineTemplate> :
	ScriptableObject,
	ITemplate
	where TRoutineTemplate :
		ITemplate,
		new()
{
	[SerializeField]
	private TRoutineTemplate template = new TRoutineTemplate();

	public TRoutineTemplate Template => this.template;

	public Func<IRoutine?> GetRoutineFnFor(GameObject agent) {
		return this.template.GetRoutineFnFor(agent);
	}
}
