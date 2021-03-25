using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class InspectorDict<TKey, TValue> : IInspectorDict<TKey, TValue>
{
	private List<Record<TKey, TValue>> records;

	public TValue this[TKey key]
	{
		get => this.Get(key).value;
		set => this.Set(key, value);
	}

	public InspectorDict(List<Record<TKey, TValue>> records) => this.records = records;

	private Record<TKey, TValue> Get(TKey key)
	{
		return this.records.OrEmpty()
			.Where(d => d.key.Equals(key))
			.FirstOrDefault();
	}

	private void Set(TKey key, TValue value)
	{
		Record<TKey, TValue> add = new Record<TKey, TValue>{ key = key, value = value };

		Action addOrUpdate = this.records.FindIndex(r => r.key.Equals(key)) switch {
			-1 => () => this.records.Add(add),
			int i => () => this.records[i] = add,
		};
		addOrUpdate();
	}
}
