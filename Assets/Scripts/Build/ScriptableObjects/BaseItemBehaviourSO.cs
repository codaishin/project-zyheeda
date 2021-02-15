using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemBehaviourSO : ScriptableObject
{
	public abstract
	bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine);
}
