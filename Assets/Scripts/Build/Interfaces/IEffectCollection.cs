using System;
using UnityEngine;

public interface IEffectCollection<TSheet>
	where TSheet: ISheet
{
	bool GetApplyEffects(TSheet source, GameObject target, out Action applyEffects);
}
