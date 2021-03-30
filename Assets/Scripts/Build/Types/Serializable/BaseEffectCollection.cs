using System;
using System.Linq;
using UnityEngine;


[Serializable]
public class BaseEffectCollection<TSheet, TEffectFactory> : IEffectCollection<TSheet>
	where TSheet : IConditionManager, ISections
	where TEffectFactory : IEffectFactory<TSheet>
{
	public EffectData<TSheet, TEffectFactory>[] effectData;

	public void Apply(TSheet source, TSheet target)
	{
		foreach (EffectData<TSheet, TEffectFactory> data in this.effectData) {
			Effect effect = data.GetEffect(source, target);
			if (effect.duration == 0) {
				effect.Apply();
				effect.Revert();
			} else {
				target.Add(effect);
			}
		}
	}
}
