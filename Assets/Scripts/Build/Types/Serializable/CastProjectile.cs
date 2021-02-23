using System;
using UnityEngine;

[Serializable]
public class CastProjectile : BaseCastProjectile<Magazine>
{
	protected override
	Movement.ApproachFunc<GameObject> Approach { get; } = Movement.GetApproach(
		getPosition: (GameObject target) => target.transform.position,
		getTimeDelta: () => Time.fixedDeltaTime,
		postUpdate: (projectile, target) => projectile.transform.LookAt(target.transform)
	);
}
