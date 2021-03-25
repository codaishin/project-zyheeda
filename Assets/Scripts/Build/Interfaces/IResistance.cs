public interface IInspectorDict<TKey, TValue>
{
	TValue this[TKey tag] { get; set; }
}
