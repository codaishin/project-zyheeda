using UnityEngine;

public abstract class BaseEffectSO : ScriptableObject
{
	public BaseItemBehaviourMB Item { get; private set; }

	public abstract void Apply(in SkillMB skill, in GameObject target);
}
