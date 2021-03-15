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

	public float this[EffectTag tag] {

		get => this.data != null
			? this.data.Where(d => d.tag == tag).FirstOrDefault().value
			: 0f;
		set {
			int hit = Array.FindIndex(this.data, d => d.tag == tag);
			if (hit == -1) {
				this.data = this.data
					.Concat(new Data[] { new Data { name = tag.ToString(), tag = tag, value = value } })
					.ToArray();
			} else {
				this.data[hit].value = value;
			}
		}
	}
}
