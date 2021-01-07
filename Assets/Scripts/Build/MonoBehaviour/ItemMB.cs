using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItemMB : MonoBehaviour
{
	public abstract
	IEnumerator<WaitForFixedUpdate> Apply(SkillMB skill, GameObject target);
}
