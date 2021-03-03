using System;
using UnityEngine;

[Serializable]
public class LogEffect : IEffectCollection<CharacterSheetMB>
{
	private static void Log(in GameObject target, in CharacterSheetMB source)
	{
		Debug.Log($"{source.name} hit {target.name}");
	}

	public bool GetHandle(GameObject target, out Action<CharacterSheetMB> handle)
	{
		handle = attributes => LogEffect.Log(target, attributes);
		return true;
	}
}
