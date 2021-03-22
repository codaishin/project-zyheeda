using NUnit.Framework;
using UnityEngine;

public class RayCastHitCharacterSheetMBTests : TestCollection
{
	[Test]
	public void GetSheet()
	{
		var rayCast = new GameObject("rayCast").AddComponent<RayCastHitCharacterSheetMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<CharacterSheetMB>();

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.AreEqual((true, sphere), (rayCast.Get(hit, out var g), g));
	}

	[Test]
	public void GetSheetFalse()
	{
		var rayCast = new GameObject("rayCast").AddComponent<RayCastHitCharacterSheetMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.False(rayCast.Get(hit, out _));
	}
}
