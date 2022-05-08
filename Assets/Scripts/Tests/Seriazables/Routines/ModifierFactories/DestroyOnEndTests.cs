using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class DestroyendTests : TestCollection
	{
		[UnityTest]
		public IEnumerator Destroyend() {
			var obj = new GameObject();
			var plugin = new DestroyOnEnd();
			var modifierFn = plugin.GetModifierFnFor(obj);
			var end = modifierFn(new Data()).end!;

			yield return new WaitForEndOfFrame();

			end();

			yield return new WaitForEndOfFrame();

			Assert.True(obj == null);
		}
	}
}
