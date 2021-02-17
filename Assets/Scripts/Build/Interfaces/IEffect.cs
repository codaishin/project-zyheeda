using UnityEngine;

public delegate void EffectFunc(in Attributes attributes);

public interface IEffect
{
	bool GetEffect(GameObject target, out EffectFunc effectHandle);
}
