using System;
using System.Linq;
using UnityEngine;


[Serializable]
public class BaseEffectCollection<TSheet, TEffectFactory> : IEffectCollection<TSheet>
	where TSheet : IConditionManager, ISections
	where TEffectFactory : IEffectFactory<TSheet>
{
	public EffectData<TSheet, TEffectFactory>[] effectData;

	private void Apply(TSheet source, TSheet target)
	{
		foreach (EffectData<TSheet, TEffectFactory> data in this.effectData) {
			Effect effect = data.GetEffect(source, target);
			if (effect.duration == 0) {
				if (effect.Apply(out Action revert)) {
					revert();
				}
			} else {
				target.Add(effect);
			}
		}
	}

	public bool GetApplyEffects(TSheet source, GameObject target, out Action applyEffects)
	{
		if (target.TryGetComponent(out TSheet targetSheet)) {
			applyEffects = () => this.Apply(source, targetSheet);
			return true;
		}
		applyEffects = default;
		return false;
	}
}
