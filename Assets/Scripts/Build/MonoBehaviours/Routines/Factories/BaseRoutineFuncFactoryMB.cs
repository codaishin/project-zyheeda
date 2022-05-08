using System;
using Routines;
using UnityEngine;

public class BaseRoutineFuncFactoryMB<TFactory> :
	MonoBehaviour,
	IFuncFactory
	where TFactory :
		IFuncFactory,
		new()
{
	[SerializeField]
	private TFactory factory = new TFactory();

	public TFactory Factory => this.factory;

	public Func<IRoutine?> GetRoutineFnFor(GameObject agent) {
		return this.factory.GetRoutineFnFor(agent);
	}
}
