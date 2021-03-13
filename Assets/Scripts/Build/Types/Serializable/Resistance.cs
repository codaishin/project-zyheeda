using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Resistance
{
	[Serializable]
	public struct Data
	{
		[HideInInspector]
		public string name;
		public EffectTag tag;
		[Range(0f, 1f)]
		public float value;
	}

	public Data[] data;

	public float this[EffectTag tag] => this.data != null
		? this.data.Where(d => d.tag == tag).FirstOrDefault().value
		: 0f;
}
