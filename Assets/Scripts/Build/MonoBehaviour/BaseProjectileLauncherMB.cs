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
			if (hitable.TryHit(skill.data.offense)) {
				this.Effects.Values.ForEach(e => e.Apply(skill, target));
			}
		}
	}

	private IEnumerator<WaitForFixedUpdate> GetIterator(in GameObject target)
	{
		return this.projectilePathing.ProjectileRoutineTo(target.transform);
	}

	protected override void Awake()
	{
		if (this.projectilePathing == null) {
			this.projectilePathing = new TProjectilePathing();
		}
		base.Awake();
	}
}
