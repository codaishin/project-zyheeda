using NUnit.Framework;
using UnityEngine;

public class RayCastHitGameObjectMBTests : TestCollection
{
	[Test]
	public void TestGetGameObject()
	{
		var rayCast = new GameObject("rayCast").AddComponent<RayCastHitGameObjectMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.AreEqual((true, sphere), (rayCast.Get(hit, out var g), g));
	}
}
