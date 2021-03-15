using System;

[Serializable]
public struct EffectData
{
	public int intensity;
	public float duration;
	public SilenceTag silence;
	public BaseEffectBehaviourSO behaviour;
}
