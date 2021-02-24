using System.Collections.Generic;
using UnityEngine;

public abstract class BaseApproach<TTarget> : IApproach<TTarget>
{
	public IEnumerator<WaitForFixedUpdate> Approach(Transform transform, TTarget target, float speed)
	{
		yield break;
	}
}
