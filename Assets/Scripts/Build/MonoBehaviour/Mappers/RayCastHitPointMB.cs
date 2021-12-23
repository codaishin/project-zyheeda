using UnityEngine;

public abstract class BaseRayCastHitPointMB<TRayCastHit> :
	BaseEventMapperMB<Vector3>
	where TRayCastHit :
		MonoBehaviour,
		IRayCastHit
{
	private TRayCastHit? rayCastHit;

	private void Start() {
		this.rayCastHit = this.RequireComponent<TRayCastHit>();
	}

	public override Maybe<Vector3> Map() {
		return this
			.rayCastHit!
			.TryHit()
			.Map(hit => Maybe.Some(hit.point));
	}
}

[RequireComponent(typeof(RayCastHitMB))]
public class RayCastHitPointMB : BaseRayCastHitPointMB<RayCastHitMB> { }
