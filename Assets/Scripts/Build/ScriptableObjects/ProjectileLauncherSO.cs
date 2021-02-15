using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerator<WaitForFixedUpdate> ProjectileRoutine(Transform agent, Transform to);

public class ProjectileLauncherSO : BaseItemBehaviourSO
{
	public ProjectileRoutine projectileRoutine;

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
		IEnumerator<WaitForFixedUpdate> projectilePath = this.projectileRoutine(null, target.transform);
		while (projectilePath.MoveNext()) {
			yield return projectilePath.Current;
		}
		if (hitable.TryHit(skill.modifiers.offense)) {
			skill.effects.ForEach(e => e.Apply(skill, target));
		}
	}
}
