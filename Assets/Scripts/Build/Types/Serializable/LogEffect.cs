using System;
using UnityEngine;

[Serializable]
public class LogEffect : IEffect, ISetGameObject, IGetGameObject
{
	public GameObject gameObject { get; set; }

	private static void Log(in GameObject target, in Attributes attributes)
	{
		Debug.Log($"Applied to {target} with: {attributes}");
	}

	public bool GetEffect(GameObject target, out EffectFunc effect)
	{
		effect = (in Attributes attributes) => LogEffect.Log(target, attributes);
		return true;
	}
}
