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
		bool matched = false;
		Func<EffectTag, bool> match = t => (!matched && (matched = t == tag));
		Func<Resistance.Data> getNewElem = () => new Resistance.Data { name = tag.ToString(), tag = tag, value = value };

		foreach (Resistance.Data origElem in data) {
			yield return match(origElem.tag) switch {
				true => getNewElem(),
				false => origElem,
			};
		}
		if (!matched) {
			yield return getNewElem();
		}
	}
}
