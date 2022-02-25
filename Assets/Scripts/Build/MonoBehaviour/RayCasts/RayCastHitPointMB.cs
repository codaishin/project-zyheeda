using UnityEngine;

[RequireComponent(typeof(RayCastHitMB))]
public class RayCastHitPointMB :
	BaseRayCastHitMapperMB<RayCastHitMB, Vector3>
{
	public override Maybe<Vector3> Map(Maybe<RaycastHit> hit) {
		return hit.Map(hit => Maybe.Some(hit.point));
	}
}