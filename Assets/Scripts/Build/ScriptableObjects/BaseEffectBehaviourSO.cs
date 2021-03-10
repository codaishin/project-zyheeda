using UnityEngine;

public abstract class BaseEffectBehaviourSO : ScriptableObject
{
	public abstract void Apply(CharacterSheetMB source, CharacterSheetMB target, int intensity);
	public abstract void Maintain(CharacterSheetMB source, CharacterSheetMB target, int intensity, float intervalDelta);
	public abstract void Revert(CharacterSheetMB source, CharacterSheetMB target, int intensity);
}
