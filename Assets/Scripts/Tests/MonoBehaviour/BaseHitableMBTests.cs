using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseHitableMBTests
{
	private class MockHitableMB : BaseHitableMB {}

	[Test]
	public void OnlyOneHitable()
	{
		var hitable = new GameObject("hitable").AddComponent<MockHitableMB>();

		hitable.gameObject.AddComponent<MockHitableMB>();

		Assert.AreEqual(1, hitable.gameObject.GetComponents<MockHitableMB>().Count());
	}
}
