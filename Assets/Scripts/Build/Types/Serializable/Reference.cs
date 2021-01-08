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

	public static implicit operator Reference(in GameObject gameObject) =>
		new Reference { gameObject = gameObject };

	public static implicit operator Reference(in Component component) =>
		new Reference { gameObject = component.gameObject };

	public static implicit operator Reference(in ReferenceSO referenceSO) =>
		new Reference { referenceSO = referenceSO };

	public static implicit operator Reference(in (GameObject, ReferenceSO) refTuple) =>
		new Reference { gameObject = refTuple.Item1, referenceSO = refTuple.Item2 };
}
