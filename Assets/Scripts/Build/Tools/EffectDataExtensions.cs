public static class EffectDataExtensions
{
	public static Effect GetEffect<TSheet>(this EffectData data, TSheet source, TSheet target)
		where TSheet : ISections
	{
		Effect effect = new Effect{ duration = data.duration };
		if (data.silence != SilenceTag.ApplyAndRevert) {
			effect.OnApply += () => data.behaviour.Apply(source, target, data.intensity);
			effect.OnRevert += () => data.behaviour.Revert(source, target, data.intensity);
		}
		if (data.silence != SilenceTag.Maintain) {
			effect.OnMaintain += d => data.behaviour.Maintain(source, target, data.intensity, d);
		}
		return effect;
	}
}
