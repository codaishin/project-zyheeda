using System;

[Serializable]
public struct EffectData<TSheet, TEffectFactory>
	where TSheet : ISections
	where TEffectFactory : IEffectFactory<TSheet>
{
	public float intensity;
	public float duration;
	public SilenceTag silence;
	public TEffectFactory factory;
}
