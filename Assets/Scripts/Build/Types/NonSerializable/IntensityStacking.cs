using System;
using System.Collections.Generic;

public class IntensityStacking : IEffectRoutineStacking
{
	public void Add(Finalizable effectRoutine, List<Finalizable> routines, Action<Finalizable> onAdd)
	{
		routines.Add(effectRoutine);
		onAdd(effectRoutine);
	}
}
