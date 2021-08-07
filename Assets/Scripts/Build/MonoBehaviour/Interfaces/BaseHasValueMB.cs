using UnityEngine;

public abstract class BaseHasValueMB<TValue> :
	MonoBehaviour,
	IHasValue<TValue>
{
	public virtual TValue Value { get; set; }
}
