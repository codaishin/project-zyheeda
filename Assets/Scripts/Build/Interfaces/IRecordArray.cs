public interface IRecordArray<TKey, TValue>
{
	Record<TKey, TValue>[] Records { get; }
	void SetNamesFromKeys(string duplicateLabel);
}
