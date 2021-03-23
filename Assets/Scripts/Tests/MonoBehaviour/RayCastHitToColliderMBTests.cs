using NUnit.Framework;
using UnityEngine;

public class RayCastHitToColliderMBTests : TestCollection
{
	[Test]
	public void GetCollider()
	{
		var morph = new GameObject("rayCast").AddComponent<RayCastHitToColliderMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.AreEqual((true, sphere.GetComponent<Collider>()), (morph.TryMorph(hit, out var g), g));
	}
}
