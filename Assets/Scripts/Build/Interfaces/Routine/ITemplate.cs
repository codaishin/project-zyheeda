using System;
using UnityEngine;

namespace Routines
{
	public interface ITemplate
	{
		Func<IRoutine?> GetRoutineFnFor(GameObject gameObject);
	}
}
