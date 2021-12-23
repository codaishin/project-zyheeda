
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseResolverMBTests : TestCollection
{
	private class MockMB : MonoBehaviour { }

	private class ResolveMockMB : BaseResolverMB<MockMB> { }

	[UnityTest]
	public IEnumerator ResolveComponent() {
		var resolved = new List<MockMB>();
		var target = new GameObject("obj");
		var mocks = new MockMB[] {
			target.AddComponent<MockMB>(),
			target.AddComponent<MockMB>(),
			target.AddComponent<MockMB>(),
		};
		var resolver = new GameObject("resolver").AddComponent<ResolveMockMB>();

		yield return new WaitForEndOfFrame();

		resolver.onResolved.AddListener(c => resolved.Add(c));
		resolver.Resolve(target);

		CollectionAssert.AreEquivalent(mocks, resolved);
	}
}
