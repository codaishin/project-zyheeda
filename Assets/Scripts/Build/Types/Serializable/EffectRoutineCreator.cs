using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRoutineCreator : IEffectRoutineCreator
{
	public float intervalDelta;

	private IEnumerator PureRoutine(Effect effect)
	{
		effect.Apply();
		while (effect.duration > 0) {
			if (effect.duration < intervalDelta) {
				intervalDelta = effect.duration;
			}
			yield return new WaitForSeconds(intervalDelta);
			effect.Maintain(intervalDelta);
		}
		effect.Revert();
	}

	public Finalizable Create(Effect effect)
	{
		return new Finalizable { wrapped = this.PureRoutine(effect) };
	}
}
