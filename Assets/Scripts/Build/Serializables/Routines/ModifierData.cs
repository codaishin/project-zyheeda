using System;

namespace Routines
{
	[Serializable]
	public struct ModifierData
	{
		public ModifierHook hook;
		public Reference<IModifierFactory> factory;
	}
}
