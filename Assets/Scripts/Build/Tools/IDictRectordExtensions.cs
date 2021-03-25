using System.Linq;
using System.Collections.Generic;

public static class IDictRecordExtensions
{
	public static
	IEnumerable<Record<TKey, TValue>> Consolidate<TKey, TValue>(this IEnumerable<Record<TKey, TValue>> data)
	{
		HashSet<TKey> track = new HashSet<TKey>();

		Record<TKey, TValue> markDuplicates(Record<TKey, TValue> record) {
			record.MarkDuplicate(track.Contains(record.key));
			track.Add(record.key);
			return record;
		}

		return data.Select(markDuplicates);
	}

	public static
	IEnumerable<Record<TKey, TValue>> AddOrUpdate<TKey, TValue>(this IEnumerable<Record<TKey, TValue>> data, Record<TKey, TValue> record)
	{
		bool matched = false;

		bool match(TKey k) => (!matched && (matched = k.Equals(record.key)));

		foreach (Record<TKey, TValue> origElem in data) {
			yield return match(origElem.key) ? record : origElem;
		}
		if (!matched) {
			yield return record;
		}
	}
}
