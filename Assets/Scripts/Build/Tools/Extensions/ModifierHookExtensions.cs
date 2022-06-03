using System;
using System.Collections.Generic;
using System.Linq;

namespace Routines
{
	public static class ModifierHookExtensions
	{
		private static ModifierFlags[] flags = (ModifierFlags[])Enum.GetValues(
			typeof(ModifierFlags)
		);

		public static IEnumerable<ModifierFlags> GetFlags(this ModifierFlags hook) {
			return ModifierHookExtensions.flags.Where(flag => hook.HasFlag(flag));
		}
	}
}
