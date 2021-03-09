using System;
using UnityEngine;

public interface IEffectCollection<TSheet>
{
	bool GetApplyEffects(TSheet source, GameObject target, out Action applyEffects);
}
