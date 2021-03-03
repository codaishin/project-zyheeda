using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectExtensions
{
	public static
	IEnumerator<WaitForSeconds> AsTimedRoutine(this Effect effect, float intervalDelta)
	{
		effect.Apply();
		while (effect.duration > 0) {
			if (effect.duration < intervalDelta) intervalDelta = effect.duration;
			yield return new WaitForSeconds(intervalDelta);
			effect.Maintain(intervalDelta);
		}
		effect.Revert();
	}
}
