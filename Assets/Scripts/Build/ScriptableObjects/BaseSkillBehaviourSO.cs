using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSkillBehaviourSO<TYield> : ScriptableObject
	where TYield: YieldInstruction
{
	public abstract
	IEnumerator<TYield> Apply(ItemMB item, SkillMB skill, GameObject target);
}

public abstract class BaseFixedUpdateSkillBehaviourSO
	: BaseSkillBehaviourSO<WaitForFixedUpdate> {}
