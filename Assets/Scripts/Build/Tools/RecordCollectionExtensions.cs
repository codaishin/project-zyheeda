using System.Linq;
using System.Collections.Generic;

public static class RecordCollectionExtensions
{
	public static
	IEnumerable<Record<TKey, TValue>> Validate<TKey, TValue>(this IEnumerable<Record<TKey, TValue>> records)
	{
		HashSet<TKey> track = new HashSet<TKey>();

		Record<TKey, TValue> markDuplicates(Record<TKey, TValue> item) {
			item.name = track.Contains(item.key) ? "__duplicate__" : item.key.ToString();
			track.Add(item.key);
			return item;
		}

		return records.Select(markDuplicates);
	}
}
