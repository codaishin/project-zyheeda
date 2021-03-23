using NUnit.Framework;
using UnityEngine;

public class RayCastHitToGameObjectMBTests : TestCollection
{
	[Test]
	public void TestGetGameObject()
	{
		var morph = new GameObject("rayCast").AddComponent<RayCastHitToGameObjectMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.AreEqual((true, sphere), (morph.TryMorph(hit, out var g), g));
	}
}
