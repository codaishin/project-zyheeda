public static class EffectDataExtensions
{
	public static
	Effect GetEffect<TSheet, TEffectFactory>(this EffectData<TSheet, TEffectFactory> data, TSheet source, TSheet target)
		where TSheet : ISections
		where TEffectFactory : IEffectFactory<TSheet>
	{
		Effect effect = data.factory.Create(source, target, data.intensity);
		effect.duration = data.duration;
		effect.silence = data.silence;
		return effect;
	}
}
