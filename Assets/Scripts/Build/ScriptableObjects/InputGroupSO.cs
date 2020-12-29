using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGroupSO : ScriptableObject
{
	public BaseInputSO inputSO;
	public InputItem[] items;

	public void Apply() => this.items.ForEach(this.ApplyItem);
	private void ApplyItem(InputItem item) => item.Apply(this.inputSO.GetKey);
}
