using System;
using UnityEngine;

public abstract class BaseUISheetInspectMB<TSheet> : MonoBehaviour
	where TSheet : ISections
{
	private Action[] callbacks;

	public BaseUIInspectorMB<Health> uIHealth;

	public void SetSheet(TSheet sheet)
	{
		this.callbacks = new Action[] {
			sheet.UseSection((ref Health health) => this.uIHealth.Set(health)),
		};
	}

	public void Clear()
	{
		this.callbacks = null;
	}

	public void Monitor()
	{
		foreach(Action action in this.callbacks.OrEmpty()) {
			action?.Invoke();
		}
	}
}
