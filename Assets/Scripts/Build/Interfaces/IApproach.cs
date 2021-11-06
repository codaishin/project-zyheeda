using System.Collections.Generic;
using UnityEngine;

public interface IApproach<TTarget>
{
	IEnumerator<WaitForFixedUpdate> Apply(
		Transform transform,
		TTarget target,
		float speed
	);
}
