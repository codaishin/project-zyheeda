using System.Linq;
using System.Collections.Generic;

public static class IDictRecordExtensions
{
	public static
	IEnumerable<TRecord> Consolidate<TRecord, TKey, TValue>(this IEnumerable<TRecord> data)
		where TRecord : IDictRecord<TKey, TValue>
	{
		HashSet<TKey> track = new HashSet<TKey>();

		TRecord markDuplicates(TRecord record) {
			record.MarkDuplicate(track.Contains(record.Key));
			track.Add(record.Key);
			return record;
		}

		return data.Select(markDuplicates);
	}

	public static
	IEnumerable<TRecord> AddOrUpdate<TRecord, TKey, TValue>(this IEnumerable<TRecord> data, TRecord record)
		where TRecord: IDictRecord<TKey, TValue>
	{
		bool matched = false;

		bool match(TKey k) => (!matched && (matched = k.Equals(record.Key)));

		foreach (TRecord origElem in data) {
			yield return match(origElem.Key) ? record : origElem;
		}
		if (!matched) {
			yield return record;
		}
	}
}
