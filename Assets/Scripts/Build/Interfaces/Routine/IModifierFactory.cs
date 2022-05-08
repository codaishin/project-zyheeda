using UnityEngine;

namespace Routines
{
	public interface IModifierFactory
	{
		Routines.ModifierFn GetModifierFnFor(GameObject agent);
	}
}
