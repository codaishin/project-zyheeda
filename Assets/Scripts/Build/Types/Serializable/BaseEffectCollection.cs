using System;
using System.Linq;
using UnityEngine;


[Serializable]
public class BaseEffectCollection<TEffectCreator, TSheet> : IEffectCollection<TSheet>
	where TSheet : IConditionTarget
	where TEffectCreator : IEffectCreator<TSheet>
{
	public TEffectCreator[] effectData;

	private void Apply(TSheet source, TSheet target)
	{
		foreach (TEffectCreator creator in this.effectData) {
			Effect effect = creator.Create(source, target);
			if (effect.duration == 0) {
				effect.Apply();
				effect.Revert();
			} else {
				target.Add(effect, creator.EffectTag, creator.StackDuration);
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
