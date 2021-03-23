using NUnit.Framework;
using UnityEngine;

public class RayCastHitToPointMBTests : TestCollection
{
	[Test]
	public void TestGetPoint()
	{
		var morph = new GameObject("rayCastHit").AddComponent<RayCastHitToPointMB>();

		Assert.AreEqual(Vector3.left, morph.DoMorph(new RaycastHit{ point = Vector3.left }));
	}
}
