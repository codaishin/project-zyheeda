using System;
using UnityEngine;

[Serializable]
public struct Reference
{
	[SerializeField]
	private GameObject? gameObject;

	[SerializeField]
	private ReferenceSO? referenceSO;

	public GameObject GameObject {
		get {
			if (this.gameObject != null) {
				return this.gameObject;
			}
			if (this.referenceSO != null) {
				return this.referenceSO.GameObject;
			}
			throw new NullReferenceException($"{this}: no reference set");
		}
	}

	public static implicit operator Reference(GameObject gameObject) {
		return new Reference { gameObject = gameObject };
	}

	public static implicit operator Reference(ReferenceSO referenceSO) {
		return new Reference { referenceSO = referenceSO };
	}
}
