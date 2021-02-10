using UnityEngine;

public abstract class BaseEffectSO : ScriptableObject
{
	public abstract void Apply(in BaseSkillMB skill, in GameObject target);
}
