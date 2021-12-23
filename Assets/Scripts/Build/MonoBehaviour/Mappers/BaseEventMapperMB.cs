using UnityEngine;
using UnityEngine.Events;

public abstract class _BaseEventMapperMB<TOut> : MonoBehaviour
{
	public UnityEvent<TOut> onValueMapped = new UnityEvent<TOut>();

	protected void RunEvent(TOut value) => this.onValueMapped.Invoke(value);
	protected void DoNothing() { }
}

public abstract class BaseEventMapperMB<TIn, TOut> : _BaseEventMapperMB<TOut>
{
	public void Apply(TIn value) =>
		this.Map(value).Match(
			some: this.RunEvent,
			none: this.DoNothing
		);

	public abstract Maybe<TOut> Map(TIn value);
}

public abstract class BaseEventMapperMB<TOut> : _BaseEventMapperMB<TOut>
{
	public void Apply() =>
		this.Map().Match(
			some: this.RunEvent,
			none: this.DoNothing
		);

	public abstract Maybe<TOut> Map();
}
