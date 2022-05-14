using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class DestroyOnEnd : IModifierFactory
	{
		public ModifierFn GetModifierFnFor(GameObject agent) {
			return _ => () => GameObject.Destroy(agent);
		}
	}
}
