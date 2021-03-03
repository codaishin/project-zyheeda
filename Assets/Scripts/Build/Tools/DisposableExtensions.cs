using System;

public static class DisposableExtensions
{
	public static
	Disposable<T> AsDisposable<T>(this T value, in Action<T> onDispose) =>
		new Disposable<T>(value, onDispose);
}
