using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Resistance : IResistance
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

	private Data Get(EffectTag tag)
	{
		return this.data.OrEmpty()
			.Where(d => d.tag == tag)
			.FirstOrDefault();
	}

	private void Set(EffectTag tag, float value)
	{
		this.data = this.data.OrEmpty()
			.AddOrUpdate(tag, value)
			.ToArray();
	}

	public float this[EffectTag tag] {
		get => this.Get(tag).value;
		set => this.Set(tag, value);
	}
}
