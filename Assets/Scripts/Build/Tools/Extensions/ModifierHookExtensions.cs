using System;
using System.Collections.Generic;
using System.Linq;

namespace Routines
{
	public static class ModifierHookExtensions
	{
		private static PluginFlags[] flags = (PluginFlags[])Enum.GetValues(
			typeof(PluginFlags)
		);

		public static IEnumerable<PluginFlags> GetFlags(this PluginFlags hook) {
			return ModifierHookExtensions.flags.Where(flag => hook.HasFlag(flag));
		}
	}
}
