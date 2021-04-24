using System;
using UnityEngine;

[Serializable]
public abstract class BasePhysicsHit<TRayProvider> : IHit
	where TRayProvider : IRay
{
	public TRayProvider rayProvider;
	public LayerMask layerConstraints;

	public bool Try<T>(T _, out T target)
	{
		if (this.TryHit(out RaycastHit hit)) {
			return hit.transform.TryGetComponent(out target);
		}
		target = default;
		return false;
	}

	private bool TryHit(out RaycastHit hit)
	{
		Vector3 origin = this.rayProvider.Ray.origin;
		Vector3 direction = this.rayProvider.Ray.direction;
		return this.layerConstraints == default
			? Physics.Raycast(origin, direction, out hit, float.PositiveInfinity)
			: Physics.Raycast(origin, direction, out hit, float.PositiveInfinity, this.layerConstraints);
	}
}
