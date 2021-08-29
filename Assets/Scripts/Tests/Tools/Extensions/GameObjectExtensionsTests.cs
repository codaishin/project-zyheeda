using NUnit.Framework;
using UnityEngine;

public class GameObjectExtensionsTests : TestCollection
{
	private class MockMB : MonoBehaviour {}

	[Test]
	public void RequireComponent()
	{
		var obj = new GameObject("obj");
		var cmp = obj.AddComponent<MockMB>();

		Assert.AreSame(cmp, obj.RequireComponent<MockMB>());
	}

	[Test]
	public void RequireComponentMissing()
	{
		var obj = new GameObject("obj");
		Assert.Throws<MissingComponentException>(() => obj.RequireComponent<MockMB>());
	}

	[Test]
	public void RequireComponentMissingMessage()
	{
		var obj = new GameObject("obj");
		try {
			obj.RequireComponent<MockMB>();
		} catch (MissingComponentException e) {
			Assert.AreEqual(
				"GameObject \"obj\" does not have a Component of type \"GameObjectExtensionsTests+MockMB\"",
				e.Message
			);
		}
	}
}
