using System;
using System.Linq;
using System.Collections.Generic;

public static class RecordListExtensions
{
	public static void Validate<TKey, TValue>(this List<Record<TKey, TValue>> records)
	{
		HashSet<TKey> track = new HashSet<TKey>();

		Record<TKey, TValue> markDuplicates(Record<TKey, TValue> item) {
			item.name = track.Contains(item.key) ? "__duplicate__" : item.key.ToString();
			track.Add(item.key);
			return item;
		}

		for (int i = 0; i < records.Count; ++i) {
			records[i] = markDuplicates(records[i]);
		}
	}
}
