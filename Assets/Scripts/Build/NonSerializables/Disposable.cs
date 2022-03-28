using System;

public class Disposable<T> : IDisposable
{
	private Action<T> onDispose;

	public T Value { get; }

	public Disposable(in T value, in Action<T> onDispose) {
		this.onDispose = onDispose;
		this.Value = value;
	}

	public void Dispose() => this.onDispose(this.Value);
}
