using System.Collections.Generic;

public interface ITargeting<TTarget, TYield>
{
	IEnumerator<TYield> Select(TTarget Source, List<TTarget> targets);
}
