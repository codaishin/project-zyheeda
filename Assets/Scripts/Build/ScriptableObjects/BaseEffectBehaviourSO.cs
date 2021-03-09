using UnityEngine;

public abstract class BaseEffectBehaviourSO : ScriptableObject
{
	public abstract void Apply(CharacterSheetMB source, CharacterSheetMB target);
	public abstract void Maintain(CharacterSheetMB source, CharacterSheetMB target, float intervalDelta);
	public abstract void Revert(CharacterSheetMB source, CharacterSheetMB target);
}
