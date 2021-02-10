using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemBehaviour
{
	public BaseEffectSO[] effects = new BaseEffectSO[0];

	public abstract
	bool Apply(BaseSkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine);
}
