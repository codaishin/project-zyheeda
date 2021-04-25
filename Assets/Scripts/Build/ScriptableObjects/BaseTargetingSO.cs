using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargetingSO : ScriptableObject, ITargeting<CharacterSheetMB, WaitForEndOfFrame>
{
	public abstract IEnumerator<WaitForEndOfFrame> Select(
		CharacterSheetMB source,
		List<CharacterSheetMB> targets,
		int maxCount = 1
	);
}
