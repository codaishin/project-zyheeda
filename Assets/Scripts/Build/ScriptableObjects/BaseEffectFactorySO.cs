using UnityEngine;

public abstract class BaseEffectFactorySO<TSheet> : ScriptableObject, IEffectFactory<TSheet>
	where TSheet : ISections
{
	public EffectTag tag;
	public ConditionStacking stacking;

	public abstract Effect Create(TSheet source, TSheet target, float intensity);
}
