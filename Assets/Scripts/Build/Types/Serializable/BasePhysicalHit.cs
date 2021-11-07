using System;
using UnityEngine;

[Serializable]
public abstract class BasePhysicsHit<TRayProvider> :
	IHit
	where TRayProvider : IRay
{
	public TRayProvider? rayProvider;
	public LayerMask layerConstraints;

	public Maybe<T> Try<T>(T _) where T : notnull {
		if (
			this.TryHit(out RaycastHit hit) &&
			hit.transform.TryGetComponent(out T target)
		) {
			return Maybe.Some(target);
		}
		return Maybe.None<T>();
	}

	private bool TryHit(out RaycastHit hit) {
		if (this.rayProvider == null) throw this.NullError();
		Vector3 origin = this.rayProvider.Ray.origin;
		Vector3 direction = this.rayProvider.Ray.direction;
		return this.layerConstraints == default
			? Physics.Raycast(origin, direction, out hit, float.PositiveInfinity)
			: Physics.Raycast(origin, direction, out hit, float.PositiveInfinity, this.layerConstraints);
	}
}
