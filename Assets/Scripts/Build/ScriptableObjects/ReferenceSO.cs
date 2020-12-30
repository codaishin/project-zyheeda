using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Reference")]
public class ReferenceSO : ScriptableObject
{
	public GameObject GameObject { get; set; }

	public void Clear() => this.GameObject = null;
}
