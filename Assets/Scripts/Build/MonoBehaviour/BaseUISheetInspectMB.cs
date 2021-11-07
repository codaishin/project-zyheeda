using System;
using UnityEngine;

public abstract class BaseUISheetInspectMB<TSheet> :
	MonoBehaviour
	where TSheet : ISections
{
	private Action[] callbacks = new Action[0];

	public BaseUIInspectorMB<Health>? uIHealth;

	public void SetSheet(TSheet sheet) {
		if (this.uIHealth == null) throw this.NullError();
		this.callbacks = new Action[] {
			sheet.UseSection((ref Health health) => this.uIHealth.Set(health)),
		};
	}

	public void Clear() {
		this.callbacks = new Action[0];
	}

	public void Monitor() {
		foreach (Action action in this.callbacks) {
			action();
		}
	}
}
