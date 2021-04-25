using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargetingSO<TSheet> : ScriptableObject, ITargeting<TSheet, WaitForEndOfFrame>
{
	public abstract
	IEnumerator<WaitForEndOfFrame> Select(TSheet source, List<TSheet> targets,int maxCount = 1);
}
