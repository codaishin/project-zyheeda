using NUnit.Framework;

namespace Routines
{
	public class ModifierHookExtensionTests
	{
		[Test]
		public
		void GetFlagsBegin() {
			var hook = PluginFlags.OnBegin;

			CollectionAssert.AreEquivalent(
				new[] { PluginFlags.OnBegin },
				hook.GetFlags()
			);
		}

		[Test]
		public
		void GetFlagsBeginAndEnd() {
			var hook = PluginFlags.OnBegin | PluginFlags.OnEnd;

			CollectionAssert.AreEquivalent(
				new[] { PluginFlags.OnBegin, PluginFlags.OnEnd },
				hook.GetFlags()
			);
		}
	}
}
