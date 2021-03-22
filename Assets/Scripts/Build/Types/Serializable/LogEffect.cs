using System;
using UnityEngine;

[Serializable]
public class LogEffect : IEffectCollection<CharacterSheetMB>
{
	public void Apply(CharacterSheetMB source, CharacterSheetMB target)
	{
		Debug.Log($"{source.name} hit {target.name}");
	}
}
