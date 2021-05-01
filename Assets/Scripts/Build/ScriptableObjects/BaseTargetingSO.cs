using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseTargetingSO<TSheet> : ScriptableObject, ITargeting<TSheet, WaitForEndOfFrame>
{
	public UnityEvent onBeginSelect;
	public UnityEvent onEndSelect;

	public IEnumerator<WaitForEndOfFrame> Select(TSheet source, List<TSheet> targets,int maxCount = 1)
	{
		IEnumerator<WaitForEndOfFrame> routine = this.DoSelect(source, targets, maxCount);
		this.onBeginSelect?.Invoke();
		while (routine.MoveNext()) {
			yield return routine.Current;
		}
		this.onEndSelect?.Invoke();
	}

	protected abstract
	IEnumerator<WaitForEndOfFrame> DoSelect(TSheet source, List<TSheet> targets,int maxCount);
}
