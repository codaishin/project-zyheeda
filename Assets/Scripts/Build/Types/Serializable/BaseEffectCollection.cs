using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class BaseEffectCollection<TEffectCreator, TSheet> : IEffectCollection<TSheet>
	where TEffectCreator : IEffectCreator<TSheet>
{
	public TEffectCreator[] effectData;

	public bool GetApplyEffects(TSheet source, GameObject target, out Action applyEffects)
	{
		if (target.TryGetComponent(out TSheet targetSheet)) {
			applyEffects = () => this.effectData
				.Select(d => d.Create(source, targetSheet))
				.ForEach(e => e.Apply());
			return true;
		}
		applyEffects = default;
		return false;
	}
}
