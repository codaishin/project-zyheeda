using System;


[Serializable]
public class BaseEffectCollection<TSheet, TEffectFactory> :
	IEffectCollection<TSheet>
		where TSheet : ISections
		where TEffectFactory : IEffectFactory
{
	public EffectData<TSheet, TEffectFactory>[] effectData;

	public void Apply(TSheet source, TSheet target)
	{
		foreach (EffectData<TSheet, TEffectFactory> data in this.effectData) {
			Effect effect = data.GetEffect(source, target);
			Action apply = effect.duration switch {
				0 => this.Run(effect),
				_ => this.Push(effect, target),
			};
			apply();
		}
	}

	private Action Run(Effect effect)
	{
		return () => {
			effect.Apply();
			effect.Revert();
		};
	}

	private Action Push(Effect effect, TSheet target)
	{
		return target.UseSection((ref IEffectRunner runner) => runner.Push(effect));
	}
}
