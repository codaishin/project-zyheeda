using System;
using UnityEngine;

[Serializable]
public class LogEffect : IEffectCollection
{
	private static void Log(in GameObject target, in Attributes attributes)
	{
		Debug.Log($"Applied to {target} with: {attributes}");
	}

	public bool GetHandle(GameObject target, out Action<Attributes> handle)
	{
		handle = attributes => LogEffect.Log(target, attributes);
		return true;
	}
}
