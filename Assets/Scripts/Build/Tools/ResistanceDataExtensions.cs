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
		data.name = data.tag >= EffectTag.Default ? data.tag.ToString() : "__unset__";
		return data;
	}

	public static
	IEnumerable<Resistance.Data> Consolidate(this IEnumerable<Resistance.Data> data)
	{
		return data
			.Select(ResistanceDataExtensions.MarkDuplicates())
			.Select(ResistanceDataExtensions.SetName);
	}
}
