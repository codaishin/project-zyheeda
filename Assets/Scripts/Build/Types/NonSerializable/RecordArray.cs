using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class RecordArray<TKey, TValue> : IRecordArray<TKey, TValue>, ISimpleDict<TKey, TValue>
{
	[SerializeField]
	private Record<TKey, TValue>[] records;

	public Record<TKey, TValue>[] Records => this.records;

	public TValue this[TKey key]
	{
		get => this.Get(key).value;
		set => this.Set(key, value);
	}

	public RecordArray(Record<TKey, TValue>[] initialState) => this.records = initialState;

	public RecordArray() : this(new Record<TKey, TValue>[0]) {}

	public void SetNames(string duplicateLabel)
	{
		this.records = this.records
			.GroupBy(r => r.key)
			.SelectMany(g => g.Select((r, i) => {
				r.name = i == 0 ? r.key.ToString() : duplicateLabel;
				return r;
			}))
			.ToArray();
	}

	private Record<TKey, TValue> Get(TKey key)
	{
		return this.records
			.Where(d => d.key.Equals(key))
			.FirstOrDefault();
	}

	private void Set(TKey key, TValue value)
	{
		Record<TKey, TValue> record = new Record<TKey, TValue>{ key = key, value = value };
		Action updateRecords = Array.FindIndex(this.records, r => r.key.Equals(key)) switch {
			-1 => () => this.records = this.records.Concat(record).ToArray(),
			int i => () => this.records[i] = record,
		};
		updateRecords();
	}
}
