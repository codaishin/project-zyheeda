using UnityEngine;

public abstract class BaseEffectSO : ScriptableObject
{
	public abstract void Apply(in SkillMB skill, in GameObject target);
}
