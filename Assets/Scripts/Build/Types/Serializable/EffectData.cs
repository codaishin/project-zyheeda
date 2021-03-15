using System;

[Serializable]
public struct EffectData
{
	public float intensity;
	public float duration;
	public SilenceTag silence;
	public BaseEffectBehaviourSO behaviour;
}
