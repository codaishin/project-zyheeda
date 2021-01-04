using UnityEngine;

public abstract class BaseSkillBehaviourSO : ScriptableObject
{
	public abstract void Apply(in GameObject agent, in GameObject target);
}
