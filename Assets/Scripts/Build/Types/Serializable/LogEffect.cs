using System;
using UnityEngine;

[Serializable]
public class LogEffect : IEffectCollection<CharacterSheetMB>
{
	private static void Log(CharacterSheetMB source, CharacterSheetMB target)
	{
		Debug.Log($"{source.name} hit {target.name}");
	}

	public bool GetApplyEffects(CharacterSheetMB source, CharacterSheetMB target, out Action handle)
	{
		handle = () => LogEffect.Log(source, target);
		return true;
	}
}
