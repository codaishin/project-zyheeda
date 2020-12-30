using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/KeyInputGroup")]
public class KeyInputGroupSO : ScriptableObject
{
	public BaseKeyInputSO inputSO;
	public KeyInputItem[] items;

	public void Apply() => this.items.ForEach(this.ApplyItem);
	private void ApplyItem(KeyInputItem item) => item.Apply(this.inputSO.GetKey);
}
