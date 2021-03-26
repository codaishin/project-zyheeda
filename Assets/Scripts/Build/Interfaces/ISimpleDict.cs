public interface ISimpleDict<TKey, TValue>
{
	TValue this[TKey tag] { get; set; }
}
