using System;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class BaseInspectorDict<TKey, TValue> : ISerializationCallbackReceiver
{
	[Serializable]
	public struct Record : IDictRecord<TKey, TValue>
	{
		[HideInInspector]
		public string name;
		public TKey key;
		public TValue value;


		public TKey Key => this.key;
		public TValue Value => this.value;

		public void MarkDuplicate(bool duplicate) => this.name = duplicate
			? "__duplicate__"
			: this.key.ToString();
	}

	[SerializeField]
	private Record[] data;

	public Record[] Data => data;

	public TValue this[TKey key]
	{
		get => this.Get(key).Value;
		set => this.Set(key, value);
	}

	public BaseInspectorDict(Record[] data) => this.data = data;

	public BaseInspectorDict() => this.data = new Record[0];

	private Record Get(TKey key)
	{
		return this.data.OrEmpty()
			.Where(d => d.Key.Equals(key))
			.FirstOrDefault();
	}

	private void Set(TKey tag, TValue value)
	{
		this.data = this.data.OrEmpty()
			.AddOrUpdate<Record, TKey, TValue>(new Record{ key = tag, value = value })
			.ToArray();
	}

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize()
	{
		this.data = this.data
			.Consolidate<Record, TKey, TValue>()
			.ToArray();
	}
}
