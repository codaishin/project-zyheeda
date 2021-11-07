using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Reference")]
public class ReferenceSO : ScriptableObject
{
	private GameObject? target;

	public GameObject GameObject {
		get => this.target ?? throw this.NullError();
		set => this.target = value;
	}

	public bool IsSet => this.target != null;

	public void Clear() => this.target = null;
}
