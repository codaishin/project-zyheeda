using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerator<WaitForFixedUpdate> ProjectileRoutineFunc(Transform from, Transform to, float deltaPerSecond);

[CreateAssetMenu(menuName="ScriptableObjects/ItemBehaviour/ProjectileLauncher")]
public class ProjectileLauncherSO : BaseItemBehaviourSO
{
	public ProjectileRoutineFunc projectileRoutine = ProjectileRoutine.Create;

	public float deltaPerSecond;

	public override
	bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
	{
		if (target.TryGetComponent(out BaseHitableMB hitable)) {
			routine = this.Apply(skill, target, hitable);
			return true;
		}
		routine = default;
		return false;
	}

	private
	IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target, BaseHitableMB hitable)
	{
		IEnumerator<WaitForFixedUpdate> projectilePath = this
			.projectileRoutine(skill.Item.transform, target.transform, this.deltaPerSecond);
		while (projectilePath.MoveNext()) {
			yield return projectilePath.Current;
		}
	}
}
