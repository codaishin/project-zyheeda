using System;

public interface IEventDict<TKey, TValue>
{
	event Action<TKey, TValue> OnAdd;
}
