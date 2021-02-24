using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseApproach<TTarget> : IApproach<TTarget>
{
	public abstract Vector3 GetPosition(in TTarget target);
	public abstract float GetTimeDelta();
	public abstract void PostUpdate(in Transform transform, in TTarget target);

	public IEnumerator<WaitForFixedUpdate> Approach(Transform transform, TTarget target, float speed)
	{
		bool notOnTarget(out Vector3 targetPosition) {
			targetPosition = this.GetPosition(target);
			return transform.position != targetPosition;
		}
		float delta = (speed > 0f ? speed : float.PositiveInfinity) * this.GetTimeDelta();

		while (notOnTarget(out Vector3 targetPosition)) {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, delta);
			this.PostUpdate(transform, target);
			yield return new WaitForFixedUpdate();
		}
	}
}
