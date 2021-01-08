using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemMB : MonoBehaviour
{
	public Dictionary<int, BaseEffectMB> Effects {
		get; private set;
	} = new Dictionary<int, BaseEffectMB>();

	public abstract
	IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target);
}
