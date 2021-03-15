using System;
using System.Linq;
using UnityEngine;


[Serializable]
public class BaseEffectCollection<TSheet> : IEffectCollection<TSheet>
	where TSheet : IConditionManager, ISections
{
	public EffectData[] effectData;

	private void Apply(TSheet source, TSheet target)
	{
		foreach (EffectData data in this.effectData) {
			Effect effect = data.GetEffect(source, target);
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
