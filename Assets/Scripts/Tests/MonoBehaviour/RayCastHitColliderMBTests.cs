using NUnit.Framework;
using UnityEngine;

public class RayCastHitColliderMBTests : TestCollection
{
	[Test]
	public void GetCollider()
	{
		var rayCast = new GameObject("rayCast").AddComponent<RayCastHitColliderMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.AreEqual((true, sphere.GetComponent<Collider>()), (rayCast.Get(hit, out var g), g));
	}
}
