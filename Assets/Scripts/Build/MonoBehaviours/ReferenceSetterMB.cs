using System;
using UnityEngine;

public class ReferenceSetterMB : MonoBehaviour
{
	public ReferenceSO? referenceSO;

	private void Start() => this.SetReference();

	private void OnDestroy() => this.referenceSO!.Clear();

	public void SetReference() {
		if (this.referenceSO == null) throw this.NullError();
		if (this.referenceSO.IsSet && referenceSO.GameObject != this.gameObject) {
			throw new ArgumentException(
				$"\"{referenceSO.name}\" already set " +
				$"to \"{referenceSO.GameObject.name}\""
			);
		}
		this.referenceSO.GameObject = this.gameObject;
	}
}
