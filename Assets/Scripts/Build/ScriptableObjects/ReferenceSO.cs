using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Reference")]
public class ReferenceSO : ScriptableObject
{
	private GameObject? gameObject;

	public GameObject GameObject {
		get => this.gameObject ?? throw this.NotSetException();
		set => this.gameObject = value;
	}

	private NullReferenceException NotSetException() {
		return new NullReferenceException($"{this} is not set with a GameObject");
	}

	public bool IsSet => this.gameObject != null;

	public void Clear() => this.gameObject = null;
}
