using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemBehaviourMB : MonoBehaviour
{
	public BaseEffectSO[] effects;

	public abstract
	bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine);

	private void Start()
	{
		if (this.effects == null) {
			this.effects = new BaseEffectSO[0];
		}
	}
}
