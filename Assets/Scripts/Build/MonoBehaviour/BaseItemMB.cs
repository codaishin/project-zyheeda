using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemMB : MonoBehaviour
{
	public Dictionary<int, BaseEffectMB> Effects { get; private set; }

	public abstract
	IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target);

	protected virtual void Awake()
	{
		this.Effects = new Dictionary<int, BaseEffectMB>();
	}
}
