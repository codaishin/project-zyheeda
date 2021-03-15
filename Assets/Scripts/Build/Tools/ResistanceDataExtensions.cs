using System;
using System.Linq;
using System.Collections.Generic;

public static class ResistanceDataExtensions
{
	private static
	Func<Resistance.Data, Resistance.Data> MarkDuplicates()
	{
		HashSet<EffectTag> track = new HashSet<EffectTag>();
		return data => {
			if (track.Contains(data.tag)) {
				data.tag = (EffectTag)(-1);
			}
			track.Add(data.tag);
			return data;
		};
	}

	private static Resistance.Data SetName(Resistance.Data data)
	{
		data.name = data.tag >= EffectTag.Physical ? data.tag.ToString() : "__unset__";
		return data;
	}

	public static
	IEnumerable<Resistance.Data> Consolidate(this IEnumerable<Resistance.Data> data)
	{
		return data
			.Select(ResistanceDataExtensions.MarkDuplicates())
			.Select(ResistanceDataExtensions.SetName);
	}

	public static
	IEnumerable<Resistance.Data> AddOrUpdate(this IEnumerable<Resistance.Data> data, EffectTag tag, float value)
	{
		bool hit = false;
		foreach (Resistance.Data d in data) {
			hit = hit || d.tag == tag;
			yield return hit
				? new Resistance.Data { name = d.name, value = value, tag = tag }
				: d;
		}
		if (!hit) {
			yield return new Resistance.Data { name = tag.ToString(), value = value, tag = tag };
		}
	}
}
