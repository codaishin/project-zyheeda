using UnityEngine;

public abstract class BaseHasValueMB<TValue> :
	MonoBehaviour,
	IHasValue<TValue>
{
	private TValue? value;

	public virtual TValue Value {
		get => this.value ?? throw this.NullError();
		set => this.value = value;
	}
}
