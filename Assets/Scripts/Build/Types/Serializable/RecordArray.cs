using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RecordArray<TKey, TValue> :
	IRecordArray<TKey, TValue>,
	ISimpleDict<TKey, TValue>,
	IEventDict<TKey, TValue>
{
	[SerializeField]
	private Record<TKey, TValue>[] records;

	public event Action<TKey, TValue> OnAdd;

	public TValue this[TKey key]
	{
		get => this.Get(key).value;
		set => this.Set(key, value);
	}

	public RecordArray(params Record<TKey, TValue>[] initialState)
	{
		this.records = initialState;
	}

	public RecordArray() : this(new Record<TKey, TValue>[0]) {}

	public void SetNamesFromKeys(string duplicateLabel)
	{
		this.records = this.records
			.GroupBy(r => r.key)
			.SelectMany(g => g.Select((r, i) => {
				r.name = i == 0 ? r.key?.ToString() : duplicateLabel;
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
		Predicate<Record<TKey, TValue>> match = r => r.key.Equals(key);
		Record<TKey, TValue> record = new Record<TKey, TValue> {
			key = key,
			value = value,
		};
		Action updateRecords = Array.FindIndex(this.records, match) switch {
			-1 => () => this.records = this.records.Concat(record).ToArray(),
			int i => () => this.records[i] = record,
		};
		updateRecords();
		this.OnAdd?.Invoke(key, value);
	}

	public IEnumerator<Record<TKey, TValue>> GetEnumerator()
	{
		foreach (Record<TKey, TValue> record in this.records) {
			yield return record;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
