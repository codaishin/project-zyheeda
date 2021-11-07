using System;
using System.Linq;
using System.Collections.Generic;

public class DurationStacking : IEffectRoutineStacking
{
	private static Action<Finalizable> Concat(Finalizable snd) => fst => fst.wrapped = fst.wrapped.Concat(snd);

	public void Add(Finalizable effectRoutine, List<Finalizable> routines, Action<Finalizable> onAdd) {
		(Action<Finalizable> apply, Finalizable wrapper) = routines.Count switch {
			0 => (routines.Add + onAdd, new Finalizable(effectRoutine)),
			_ => (DurationStacking.Concat(effectRoutine), routines.First()),
		};
		apply(wrapper);
	}
}
