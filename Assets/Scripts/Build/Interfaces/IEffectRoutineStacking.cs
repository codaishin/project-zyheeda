using System;
using System.Collections.Generic;

public interface IEffectRoutineStacking
{
	void Add(
		Finalizable effectRoutine,
		List<Finalizable> routines,
		Action<Finalizable> onAdd
	);
}
