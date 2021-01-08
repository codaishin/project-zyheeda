using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileLauncherMB<TProjectilePathing> : BaseItemMB
	where TProjectilePathing: IProjectilePathing
{
	public override
	IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target)
	{
		yield break;
	}
}
