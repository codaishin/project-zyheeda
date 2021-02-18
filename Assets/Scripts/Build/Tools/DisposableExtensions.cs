public static class DisposableExtensions
{
	public static
	Disposable<T> AsDisposable<T>(this T value, in OnDisposeFunc<T> onDispose) =>
		new Disposable<T>(value, onDispose);

	public static
	Disposable<T> Use<T>(this Disposable<T> disposable, out T value)
	{
		value = disposable.Value;
		return disposable;
	}
}
