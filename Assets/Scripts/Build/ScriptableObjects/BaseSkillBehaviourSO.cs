using System.Collections;
using UnityEngine;

public abstract class BaseSkillBehaviourSO : ScriptableObject
{
	public abstract
	IEnumerator Apply(CharacterMB agent, SkillMB skill, GameObject target);
}
