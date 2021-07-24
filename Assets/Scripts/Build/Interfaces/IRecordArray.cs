using System.Collections.Generic;

public interface IRecordArray<TKey, TValue> : IEnumerable<Record<TKey, TValue>>
{
	void SetNamesFromKeys(string duplicateLabel);
}
