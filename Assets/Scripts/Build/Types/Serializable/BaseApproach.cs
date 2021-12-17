using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseApproach<TTarget> : IApproach<TTarget>
{

	public abstract Vector3 GetPosition(in TTarget target);
	public abstract float GetTimeDelta();
	public abstract void OnPositionUpdated(
		in Transform current,
		in TTarget target
	);

	public IEnumerator<WaitForFixedUpdate> Apply(
		Transform current,
		TTarget target,
		float speed
	) {
		foreach (Vector3 position in this.Path(current, target, speed)) {
			current.position = position;
			this.OnPositionUpdated(current, target);
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerable<Vector3> Path(
		Transform current,
		TTarget target,
		float speed
	) {
		Func<Vector3, Vector3> next = this.GetNextFunc(current, speed);

		Vector3 targetPosition = next(this.GetPosition(target));
		while (targetPosition != current.position) {
			yield return targetPosition;
			targetPosition = next(this.GetPosition(target));
		}
	}

	private Func<Vector3, Vector3> GetNextFunc(Transform current, float speed) {
		if (speed == 0) {
			return (targetPosition) => targetPosition;
		}

		float delta = speed * this.GetTimeDelta();
		return (targetPosition) => Vector3.MoveTowards(
			current.position,
			targetPosition,
			delta
		);
	}
}
