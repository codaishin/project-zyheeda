using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseTargetingSO<TSheet> :
	ScriptableObject,
	ITargeting<TSheet, WaitForEndOfFrame>
{
	public UnityEvent? onBeginSelect;
	public UnityEvent? onEndSelect;

	public IEnumerable<WaitForEndOfFrame> Select(
		TSheet source,
		List<TSheet> targets,
		int maxCount = 1
	) {
		IEnumerable<WaitForEndOfFrame> routine = this.DoSelect(
			source,
			targets,
			maxCount
		);
		this.onBeginSelect?.Invoke();
		foreach (WaitForEndOfFrame wait in routine) {
			yield return wait;
		}
		this.onEndSelect?.Invoke();
	}

	protected abstract IEnumerable<WaitForEndOfFrame> DoSelect(
		TSheet source,
		List<TSheet> targets,
		int maxCount
	);
}
