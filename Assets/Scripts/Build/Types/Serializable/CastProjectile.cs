using System;
using UnityEngine;

public class CastProjectileApproach : BaseApproach<CharacterSheetMB>
{
	public override Vector3 GetPosition(
		in CharacterSheetMB target
	) => target.transform.position;
	public override float GetTimeDelta() => Time.fixedDeltaTime;
	public override void OnPositionUpdated(
		in Transform current,
		in CharacterSheetMB target
	) => current.LookAt(target.transform);
}

[Serializable]
public class CastProjectile :
	BaseCastProjectile<Magazine, CastProjectileApproach, CharacterSheetMB>
{ }
