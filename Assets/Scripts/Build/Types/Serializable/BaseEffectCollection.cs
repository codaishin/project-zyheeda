using System;
using System.Linq;
using UnityEngine;


[Serializable]
public class BaseEffectCollection<TEffectData, TSheet> : IEffectCollection<TSheet>
	where TSheet : IConditionManager
	where TEffectData : IEffectData<TSheet>
{
	public TEffectData[] effectData;

	private void Apply(TSheet source, TSheet target)
	{
		foreach (TEffectData creator in this.effectData) {
			Effect effect = creator.GetEffect(source, target);
			if (effect.duration == 0) {
				effect.Apply();
				effect.Revert();
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
