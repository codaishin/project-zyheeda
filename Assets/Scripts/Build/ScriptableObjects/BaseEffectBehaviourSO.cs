using UnityEngine;

public abstract class BaseEffectBehaviourSO : ScriptableObject
{
	public EffectTag tag;
	public ConditionStacking stacking;

	public abstract void Apply<TSheet>(TSheet source, TSheet target, float intensity)
		where TSheet : ISections;

	public abstract void Maintain<TSheet>(TSheet source, TSheet target, float intensity, float intervalDelta)
		where TSheet : ISections;

	public abstract void Revert<TSheet>(TSheet source, TSheet target, float intensity)
		where TSheet : ISections;
}
