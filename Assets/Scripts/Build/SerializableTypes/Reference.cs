using System;
using UnityEngine;

[Serializable]
public struct Reference
{
	[SerializeField]
	private GameObject gameObject;

	[SerializeField]
	private ReferenceSO referenceSO;

	public GameObject GameObject
	{
		get {
			if (this.gameObject) {
				if (this.referenceSO) {
					throw new ArgumentException("\"gameObject\" and \"referenceSO\" are both set");
				}
				return this.gameObject;
			}
			return this.referenceSO.GameObject;
		}
	}

	public Reference(in GameObject gameObject)
	{
		this.gameObject = gameObject;
		this.referenceSO = default;
	}

	public Reference(in ReferenceSO referenceSO)
	{
		this.gameObject = default;
		this.referenceSO = referenceSO;
	}

	public Reference(in GameObject gameObject, in ReferenceSO referenceSO)
	{
		this.gameObject = gameObject;
		this.referenceSO = referenceSO;
	}
}
