using System;
using System.Linq;

public class RecordsArray<TKey, TValue> : ISimpleDict<TKey, TValue>
{
	private Func<Record<TKey, TValue>[]> getRecords;
	private Action<Record<TKey, TValue>[]> setRecords;

	public TValue this[TKey key]
	{
		get => this.Get(key).value;
		set => this.Set(key, value);
	}

	public RecordsArray(Func<Record<TKey, TValue>[]> get, Action<Record<TKey, TValue>[]> set)
	{
		this.getRecords = get;
		this.setRecords = set;
	}

	private Record<TKey, TValue> Get(TKey key)
	{
		return this.getRecords().OrEmpty()
			.Where(d => d.key.Equals(key))
			.FirstOrDefault();
	}

	private void Set(TKey key, TValue value)
	{
		Record<TKey, TValue> record = new Record<TKey, TValue>{ key = key, value = value };
		Record<TKey, TValue>[] records = this.getRecords();
		Action updateRecords = Array.FindIndex(records, r => r.key.Equals(key)) switch {
			-1 => () => records = records.Concat(record).ToArray(),
			int i => () => records[i] = record,
		};
		updateRecords();
		this.setRecords(records);
	}
}
