using UnityEngine;

[RequireComponent(typeof(RayCastHitMB))]
public class RayCastHitGameObjectMB :
	BaseRayCastHitMapperMB<RayCastHitMB, GameObject>
{
	public override Maybe<GameObject> Map(Maybe<RaycastHit> hit) {
		return hit.Map(hit => Maybe.Some(hit.transform.gameObject));
	}
}
