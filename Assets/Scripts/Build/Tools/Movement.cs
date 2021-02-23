using System;
using System.Collections.Generic;
using UnityEngine;

public static class Movement
{
	public delegate IEnumerator<WaitForFixedUpdate> TowardsFunc<TTarget>(
		Transform transform,
		TTarget target,
		float deltaPerSecond
	);

	public delegate TowardsFunc<TTarget> GetTowardsFunc<TTarget>(
		Func<TTarget, Vector3> getPosition,
		Func<float> getTimeDelta
	);

	private static
	IEnumerator<WaitForFixedUpdate> Towards<TTarget>(
		Func<TTarget, Vector3> getPosition,
		Func<float> getTimeDelta,
		Transform transform,
		TTarget targetObject,
		float deltaPerSecond
	) {
		bool notOnTarget(out Vector3 target) {
			target = getPosition(targetObject);
			return transform.position != target;
		}

		float delta = (deltaPerSecond > 0f ? deltaPerSecond : float.PositiveInfinity) * getTimeDelta();

		while (notOnTarget(out Vector3 target)) {
			transform.position = Vector3.MoveTowards(transform.position, target, delta);
			yield return new WaitForFixedUpdate();
		}
	}

	public static TowardsFunc<TTarget> Towards<TTarget>(
		Func<TTarget, Vector3> getPosition,
		Func<float> getTimeDelta
	) => (transform, target, deltaPerSecond) => Movement
		.Towards(getPosition, getTimeDelta, transform, target, deltaPerSecond);
}
