using System;
using System.Collections.Generic;
using UnityEngine;

public static class Movement
{
	public delegate IEnumerator<WaitForFixedUpdate> ApproachFunc<TTarget>(
		Transform transform,
		TTarget target,
		float deltaPerSecond
	);

	public static ApproachFunc<TTarget> GetApproach<TTarget>(
		Func<TTarget, Vector3> getPosition,
		Func<float> getTimeDelta,
		params Action<Transform, TTarget>[] postUpdate
	) {
		IEnumerator<WaitForFixedUpdate> approach(Transform transform, TTarget targetObject, float deltaPerSecond) {
			bool notOnTarget(out Vector3 target) {
				target = getPosition(targetObject);
				return transform.position != target;
			}
			float delta = (deltaPerSecond > 0f ? deltaPerSecond : float.PositiveInfinity) * getTimeDelta();

			while (notOnTarget(out Vector3 target)) {
				transform.position = Vector3.MoveTowards(transform.position, target, delta);
				postUpdate.ForEach(update => update(transform, targetObject));
				yield return new WaitForFixedUpdate();
			}
		}
		return approach;
	}
}
