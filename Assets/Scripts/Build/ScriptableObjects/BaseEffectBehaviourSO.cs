using UnityEngine;

public abstract class BaseEffectBehaviourSO : ScriptableObject
{
	public EffectTag tag;
	public ConditionStacking stacking;

	public abstract void Apply<TSheet>(TSheet source, TSheet target, int intensity)
		where TSheet : ISections;

	public abstract void Maintain<TSheet>(TSheet source, TSheet target, int intensity, float intervalDelta)
		where TSheet : ISections;

	public abstract void Revert<TSheet>(TSheet source, TSheet target, int intensity)
		where TSheet : ISections;
}
