using NUnit.Framework;
using UnityEngine;

public class RayCastHitVector3MBTests : TestCollection
{
  [Test]
	public void TestGetPoint()
	{
		var rayCastHit = new GameObject("rayCastHit").AddComponent<RayCastHitVector3MB>();
		Assert.AreEqual(
			Vector3.left,
			rayCastHit.Get(new RaycastHit{ point = Vector3.left })
		);
	}
}
