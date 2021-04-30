using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargetingSO<TSheet> : ScriptableObject, ITargeting<TSheet, WaitForEndOfFrame>
{
	public IEnumerator<WaitForEndOfFrame> Select(TSheet source, List<TSheet> targets,int maxCount = 1)
	{
		return this.DoSelect(source, targets, maxCount);
	}

	protected abstract
	IEnumerator<WaitForEndOfFrame> DoSelect(TSheet source, List<TSheet> targets,int maxCount);
}
