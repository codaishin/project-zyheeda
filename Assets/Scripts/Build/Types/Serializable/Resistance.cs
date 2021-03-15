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

	private float Get(EffectTag tag)
	{
		return (this.data ?? new Data[0])
			.Where(d => d.tag == tag)
			.FirstOrDefault().value;
	}

	private void Set(EffectTag tag, float value)
	{
		this.data = (this.data ?? new Data[0])
			.AddOrUpdate(tag, value)
			.ToArray();
	}

	public float this[EffectTag tag] {
		get => this.Get(tag);
		set => this.Set(tag, value);
	}
}
