using System.Collections.Generic;
using UnityEngine;

public interface IItemBehaviour
{
	bool Apply(BaseSkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine);
}
