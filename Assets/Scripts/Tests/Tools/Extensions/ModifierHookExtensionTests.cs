using NUnit.Framework;

namespace Routines
{
	public class ModifierHookExtensionTests
	{
		[Test]
		public
		void GetFlagsBegin() {
			var hook = ModifierFlags.OnBegin;

			CollectionAssert.AreEquivalent(
				new[] { ModifierFlags.OnBegin },
				hook.GetFlags()
			);
		}

		[Test]
		public
		void GetFlagsBeginAndEnd() {
			var hook = ModifierFlags.OnBegin | ModifierFlags.OnEnd;

			CollectionAssert.AreEquivalent(
				new[] { ModifierFlags.OnBegin, ModifierFlags.OnEnd },
				hook.GetFlags()
			);
		}
	}
}
