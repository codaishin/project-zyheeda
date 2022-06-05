using System;

namespace Routines
{
	[Flags]
	public enum PluginFlags
	{
		OnBeginSubRoutine = 1,
		OnUpdateSubRoutine = 2,
		OnEndSubroutine = 4,
		OnBegin = 8,
		OnEnd = 16,
	}
}
