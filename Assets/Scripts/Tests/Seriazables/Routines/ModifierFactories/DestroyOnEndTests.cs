using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Routines
{
	public class DestroyendTests : TestCollection
	{
		[UnityTest]
		public IEnumerator Destroy() {
			var obj = new GameObject();
			var plugin = new DestroyOnEnd();
			var modifierFn = plugin.GetModifierFnFor(obj);
			var destroy = modifierFn(new RoutineData())!;

			yield return new WaitForEndOfFrame();

			destroy();

			yield return new WaitForEndOfFrame();

			Assert.True(obj == null);
		}
	}
}
