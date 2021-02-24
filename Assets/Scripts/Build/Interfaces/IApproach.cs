using System;
using System.Collections.Generic;
using UnityEngine;

public interface IApproach<TTarget>
{
	IEnumerator<WaitForFixedUpdate> Approach(Transform transform, TTarget target, float speed);
}
