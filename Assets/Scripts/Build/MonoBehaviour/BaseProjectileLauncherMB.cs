using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileLauncherMB<TProjectilePathing> : BaseItemMB
	where TProjectilePathing: IProjectilePathing, new()
{
	public TProjectilePathing projectilePathing;

	public override
	IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target)
	{
		if (target.TryGetComponent(out BaseHitableMB hitable)) {
			IEnumerator<WaitForFixedUpdate> iterator = this.GetIterator(target);
			while (iterator.MoveNext()) {
				yield return iterator.Current;
			}
			hitable.TryHit(skill.data.offense);
		}
	}

	private IEnumerator<WaitForFixedUpdate> GetIterator(in GameObject target) =>
		this.projectilePathing.ProjectileRoutineTo(target.transform);

	private void Awake()
	{
		this.projectilePathing = new TProjectilePathing();
	}
}
