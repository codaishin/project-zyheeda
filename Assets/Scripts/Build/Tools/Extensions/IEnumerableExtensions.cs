using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
	public static void ForEach<T>(
		this IEnumerable<T> enumerable,
		Action<T> action
	) {
		foreach (T elem in enumerable) {
			action(elem);
		}
	}

	public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? enumerable) {
		return enumerable ?? Enumerable.Empty<T>();
	}

	public static IEnumerable<T> Concat<T>(
		this IEnumerable<T> enumerable,
		T value
	) {
		foreach (T elem in enumerable) {
			yield return elem;
		}
		yield return value;
	}

	public static void Apply(this IEnumerable<Action> enumerable) {
		foreach (Action action in enumerable) {
			action();
		}
	}

	public static IEnumerable<TValue> Values<TValue>(
		this IEnumerable<Reference<TValue>> references
	) where TValue : class {
		foreach (Reference<TValue> reference in references) {
			yield return reference.Value!;
		}
	}
}
