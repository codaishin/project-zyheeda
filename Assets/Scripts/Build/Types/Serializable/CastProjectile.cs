using System;
using System.Linq;
using UnityEngine;

public class CastProjectileApproach : BaseApproach<GameObject>
{
	public override Vector3 GetPosition(in GameObject target) =>
		target.transform.position;
	public override float GetTimeDelta() =>
		Time.fixedDeltaTime;
	public override void PostUpdate(in Transform transform, in GameObject target) =>
		transform.LookAt(target.transform);
}

[Serializable]
public class CastProjectile : BaseCastProjectile<Magazine, CastProjectileApproach> {}
