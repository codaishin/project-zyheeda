public interface IRecordArray<TKey, TValue>
{
	TValue this[TKey key] {get; set; }
	Record<TKey, TValue>[] Records { get; }
	void SetNames(string duplicateLabel);
}
