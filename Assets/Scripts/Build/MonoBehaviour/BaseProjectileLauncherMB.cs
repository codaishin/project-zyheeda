using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileLauncherMB<TProjectilePathing> : BaseItemMB
	where TProjectilePathing: IProjectilePathing
{
	public TProjectilePathing projectilePathing;

	public override
	IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target)
	{
		if (target.gameObject.TryGetComponent(out BaseHitableMB hitable)) {
			hitable.TryHit(skill.data.offense);
		}
		yield break;
	}
}
