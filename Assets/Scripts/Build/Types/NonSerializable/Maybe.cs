using System;

public static class Maybe
{
	public static Some<T> Some<T>(T value) => new Some<T>(value);

	public static None<T> None<T>() => new None<T>();
}

public abstract class Maybe<T>
{
	public abstract void Match(Action<T> some, Action none);

	public abstract Maybe<TOut> Map<TOut>(Func<T, Maybe<TOut>> map);
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

	public override Maybe<TOut> Map<TOut>(Func<T, Maybe<TOut>> map) {
		return map(this.Value);
	}
}

public sealed class None<T> : Maybe<T>
{
	public override void Match(Action<T> _, Action none) {
		none();
	}

	public override Maybe<TOut> Map<TOut>(Func<T, Maybe<TOut>> map) {
		return Maybe.None<TOut>();
	}
}
