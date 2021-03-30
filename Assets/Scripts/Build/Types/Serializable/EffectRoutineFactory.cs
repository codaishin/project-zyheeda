using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class EffectRoutineFactory : IEffectRoutineFactory
{
	private class RevertPtr
	{
		public Action invoke;
	}

	public float intervalDelta = 1f;

	private IEnumerator PureRoutine(Effect effect)
	{
		float delta = this.intervalDelta;
		effect.Apply();
		while (effect.duration > 0) {
			if (effect.duration < delta) {
				delta = effect.duration;
			}
			yield return new WaitForSeconds(delta);
			effect.Maintain(delta);
		}
		effect.Revert();
	}

	public Finalizable Create(Effect effect)
	{
		return new Finalizable { wrapped = this.PureRoutine(effect) };
	}
}
