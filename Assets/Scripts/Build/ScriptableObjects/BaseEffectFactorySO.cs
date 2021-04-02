using UnityEngine;

public abstract class BaseEffectFactorySO : ScriptableObject, IEffectFactory
{
	public EffectTag tag;
	public ConditionStacking stacking;

	public abstract Effect Create<TSheet>(TSheet source, TSheet target, float intensity)
		where TSheet : ISections;
}
