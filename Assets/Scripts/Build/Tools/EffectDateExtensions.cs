public static class EffectDataExtensions
{
	public static Effect GetEffect<TSheet>(this EffectData data, TSheet source, TSheet target)
		where TSheet : ISections
	{
		Effect effect = new Effect{ duration = data.duration };
		effect.OnApply += () => data.behaviour.Apply(source, target, data.intensity);
		effect.OnMaintain += d => data.behaviour.Maintain(source, target, data.intensity, d);
		effect.OnRevert += () => data.behaviour.Revert(source, target, data.intensity);
		return effect;
	}
}
