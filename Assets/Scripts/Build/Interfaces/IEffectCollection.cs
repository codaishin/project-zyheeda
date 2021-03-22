using System;

public interface IEffectCollection<TSheet>
{
	bool GetApplyEffects(TSheet source, TSheet target, out Action applyEffects);
}
