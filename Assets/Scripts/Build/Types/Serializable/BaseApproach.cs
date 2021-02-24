using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseApproach<TTarget> : IApproach<TTarget>
{
	public Action<Transform, TTarget>[] postUpdate = new Action<Transform, TTarget>[0];

	protected abstract Vector3 GetPosition(in TTarget target);
	protected abstract float GetTimeDelta();

	public IEnumerator<WaitForFixedUpdate> Approach(Transform transform, TTarget target, float speed)
	{
		bool notOnTarget(out Vector3 targetPosition) {
			targetPosition = this.GetPosition(target);
			return transform.position != targetPosition;
		}
		float delta = (speed > 0f ? speed : float.PositiveInfinity) * this.GetTimeDelta();

		while (notOnTarget(out Vector3 targetPosition)) {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, delta);
			this.postUpdate.ForEach(cb => cb(transform, target));
			yield return new WaitForFixedUpdate();
		}
	}
}
