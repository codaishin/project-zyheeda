using System;
using System.Linq;
using System.Collections.Generic;

public static class IEnumerableExtensions
{
	public static
	void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
	{
		foreach (T elem in enumerable) {
			action(elem);
		}
	}

	public static
	IEnumerable<T> OrEmpty<T>(this IEnumerable<T> enumerable)
	{
		return enumerable ?? Enumerable.Empty<T>();
	}
}
