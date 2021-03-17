using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectRoutineCreator : IEffectRoutineCreator
{
	public float intervalDelta = 1f;

	private IEnumerator PureRoutine(Effect effect)
	{
		effect.Apply(out Action revert);
		while (effect.duration > 0) {
			if (effect.duration < intervalDelta) {
				intervalDelta = effect.duration;
			}
			yield return new WaitForSeconds(intervalDelta);
			effect.Maintain(intervalDelta);
		}
		revert();
	}

	public Finalizable Create(Effect effect)
	{
		return new Finalizable { wrapped = this.PureRoutine(effect) };
	}
}
