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

	private IEnumerator PureRoutine(Effect effect, RevertPtr revertPtr)
	{
		float delta = this.intervalDelta;
		if(effect.Apply(out Action revert)) {
			revertPtr.invoke = revert;
		};
		while (effect.duration > 0) {
			if (effect.duration < delta) {
				delta = effect.duration;
			}
			yield return new WaitForSeconds(delta);
			effect.Maintain(delta);
		}
		revertPtr.invoke();
	}

	public Finalizable Create(Effect effect, out Action revert)
	{
		RevertPtr revertPtr = new RevertPtr{ invoke = () => {} };
		revert = () => revertPtr.invoke();
		return new Finalizable { wrapped = this.PureRoutine(effect, revertPtr) };
	}
}
