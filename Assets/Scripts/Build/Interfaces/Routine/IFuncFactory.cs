using System;
using UnityEngine;

namespace Routines
{
	public interface IFuncFactory
	{
		Func<IRoutine?> GetRoutineFnFor(GameObject gameObject);
	}
}
