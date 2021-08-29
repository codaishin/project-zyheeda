using System.Collections.Generic;

public interface ITargeting<TTarget, TYield>
{
	IEnumerable<TYield> Select(
		TTarget source,
		List<TTarget> targets,
		int maxCount = 1
	);
}
