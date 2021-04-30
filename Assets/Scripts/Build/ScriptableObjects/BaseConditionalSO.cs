using UnityEngine;

public abstract class BaseConditionalSO<TValue> : ScriptableObject, IConditional<TValue>
{
	public abstract bool Check(TValue value);
}
