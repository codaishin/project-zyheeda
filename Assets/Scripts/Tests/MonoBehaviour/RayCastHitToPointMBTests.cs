using NUnit.Framework;
using UnityEngine;

public class RayCastHitToPointMBTests : TestCollection
{
	[Test]
	public void TestGetPoint()
	{
		var morph = new GameObject("rayCastHit").AddComponent<RayCastHitToPointMB>();
		Assert.AreEqual(
			(true, Vector3.left),
			(morph.TryMorph(new RaycastHit{ point = Vector3.left }, out var g), g)
		);
	}
}
