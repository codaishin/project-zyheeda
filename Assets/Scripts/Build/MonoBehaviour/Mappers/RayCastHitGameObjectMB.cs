using UnityEngine;

public class BaseRayCastHitGameObjectMB<TRayCastHit> :
	BaseEventMapperMB<GameObject>
	where TRayCastHit :
		MonoBehaviour,
		IRayCastHit
{
	private TRayCastHit? rayCastHit;

	private void Start() {
		this.rayCastHit = this.RequireComponent<TRayCastHit>();
	}

	public override Maybe<GameObject> Map() {
		return this.rayCastHit!
			.TryHit()
			.Map(hit => Maybe.Some(hit.transform.gameObject));
	}
}

[RequireComponent(typeof(RayCastHitMB))]
public class RayCastHitGameObjectMB : BaseRayCastHitGameObjectMB<RayCastHitMB>
{ }
