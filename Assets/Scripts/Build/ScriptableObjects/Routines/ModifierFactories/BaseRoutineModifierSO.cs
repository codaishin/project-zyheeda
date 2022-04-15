using Routines;
using UnityEngine;

public abstract class BaseRoutineModifierSO<TModifier> :
	ScriptableObject,
	IModifierFactory
	where TModifier :
		IModifierFactory,
		new()
{
	[SerializeField]
	protected TModifier modifier = new TModifier();

	public ModifierFn GetModifierFnFor(GameObject agent) {
		return this.modifier.GetModifierFnFor(agent);
	}
}
