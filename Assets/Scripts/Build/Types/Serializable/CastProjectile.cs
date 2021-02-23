using System;
using UnityEngine;

[Serializable]
public class CastProjectile : BaseCastProjectile<Magazine>
{
	protected override
	Movement.ApproachFunc<GameObject> Approach { get; } = Movement.GetApproach(
		(GameObject target) => target.transform.position,
		() => Time.fixedDeltaTime,
		(projectile, target) => projectile.transform.LookAt(target.transform)
	);
}
