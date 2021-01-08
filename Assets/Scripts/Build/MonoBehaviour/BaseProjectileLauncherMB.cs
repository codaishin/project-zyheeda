using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileLauncherMB<TProjectilePathing> : BaseItemMB
	where TProjectilePathing: IProjectilePathing, new()
{
	public TProjectilePathing projectilePathing;

	public override
	bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
	{
		if (target.TryGetComponent(out BaseHitableMB hitable)) {
			routine = this.Apply(target, skill, hitable);
			return true;
		}
		routine = default;
		return false;
	}

	private
	IEnumerator<WaitForFixedUpdate> Apply(GameObject target, SkillMB skill, BaseHitableMB hitable)
	{
		IEnumerator<WaitForFixedUpdate> projectilePath = this.GetProjectilePathTo(target);
		while (projectilePath.MoveNext()) {
			yield return projectilePath.Current;
		}
		if (hitable.TryHit(skill.data.offense)) {
			this.Effects.Values.ForEach(e => e.Apply(skill, target));
		}
	}

	private
	IEnumerator<WaitForFixedUpdate> GetProjectilePathTo(in GameObject target)
	{
		return this.projectilePathing.ProjectileRoutineTo(target.transform);
	}

	private void Awake()
	{
		if (this.projectilePathing == null) {
			this.projectilePathing = new TProjectilePathing();
		}
	}
}
