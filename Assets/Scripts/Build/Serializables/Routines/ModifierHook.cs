using System;

namespace Routines
{
	[Flags]
	public enum ModifierHook
	{
		OnBegin = 1,
		OnUpdate = 2,
		OnEnd = 4,
	}
}
