using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectCreator
{
	EffectTag EffectTag { get; }
	Effect Create(CharacterSheetMB source, CharacterSheetMB target);
}
