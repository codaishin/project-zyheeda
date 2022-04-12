using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DestroyOnEndTests : TestCollection
{
	[UnityTest]
	public IEnumerator DestroyOnEnd() {
		var obj = new GameObject();
		var plugin = new DestroyOnEnd();
		var callbacks = plugin.PluginHooksFor(obj);
		var onEnd = callbacks(new PluginData()).onEnd!;

		yield return new WaitForEndOfFrame();

		onEnd();

		yield return new WaitForEndOfFrame();

		Assert.True(obj == null);
	}
}
