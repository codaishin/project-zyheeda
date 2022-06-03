using System;

namespace Routines
{
	[Serializable]
	public struct ModifierData
	{
		public ModifierFlags hook;
		public Reference<IModifierFactory> factory;
	}
}
