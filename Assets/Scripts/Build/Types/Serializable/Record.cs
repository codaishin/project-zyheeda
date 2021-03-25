using System;
using UnityEngine;

[Serializable]
public struct Record<TKey, TValue>
{
	[HideInInspector]
	public string name;
	public TKey key;
	public TValue value;

	public void MarkDuplicate(bool duplicate) => this.name = duplicate
		? "__duplicate__"
		: this.key.ToString();
}
