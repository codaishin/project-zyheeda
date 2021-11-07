using System;

public static class Maybe
{
	public static Some<T> Some<T>(T value) => new Some<T>(value);

	public static None<T> None<T>() => new None<T>();
}

public abstract class Maybe<T>
{
	public abstract void Match(Action<T> some, Action none);
}

public sealed class Some<T> : Maybe<T>
{
	public T Value { get; }

	public Some(T value) {
		this.Value = value;
	}

	public override void Match(Action<T> some, Action _) {
		some(this.Value);
	}
}

public sealed class None<T> : Maybe<T>
{
	public override void Match(Action<T> _, Action none) {
		none();
	}
}
