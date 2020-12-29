using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/InputGroup")]
public class InputGroupSO : ScriptableObject
{
	public BaseInputSO inputSO;
	public InputItem[] items;

	public void Apply() => this.items.ForEach(this.ApplyItem);
	private void ApplyItem(InputItem item) => item.Apply(this.inputSO.GetKey);
}
