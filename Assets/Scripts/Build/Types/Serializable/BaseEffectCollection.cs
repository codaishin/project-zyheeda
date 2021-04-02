using System;
using System.Linq;
using UnityEngine;


[Serializable]
public class BaseEffectCollection<TSheet, TRunner, TEffectFactory> : IEffectCollection<TSheet>
	where TSheet : ISections
	where TRunner : IEffectRunner
	where TEffectFactory : IEffectFactory<TSheet>
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
		return target.UseSection((ref TRunner runner) => runner[effect.tag, effect.stacking].Push(effect));
	}
}
