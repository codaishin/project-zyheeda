using System;
using UnityEngine;

[Serializable]
public struct Record<TKey, TValue>
{
	[HideInInspector]
	public string name;
	public TKey key;
	public TValue value;
}
