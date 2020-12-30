using System;
using UnityEngine;

public class ReferenceSetterMB : MonoBehaviour
{
	public ReferenceSO referenceSO;

	private void Start() => this.SetReference();

	public void SetReference()
	{
		if (this.referenceSO.GameObject) {
			if (this.referenceSO.GameObject != this.gameObject) {
				throw new ArgumentException(
					$"\"{this.referenceSO.name}\" already set to \"{this.referenceSO.GameObject.name}\""
				);
			}
		} else {
			this.referenceSO.GameObject = this.gameObject;
		}
	}
}
