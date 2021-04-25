using System.Collections.Generic;

public interface ITargeting<TTarget, TYield>
{
	IEnumerator<TYield> Select(TTarget source, List<TTarget> targets, int maxCount = 1);
}
