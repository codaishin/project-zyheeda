using NUnit.Framework;
using UnityEngine;

public class RayCastHitToCharacterSheetMBTests : TestCollection
{
	[Test]
	public void GetSheet()
	{
		var morph = new GameObject("rayCast").AddComponent<RayCastHitToCharacterSheetMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<CharacterSheetMB>();

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.AreEqual((true, sphere), (morph.TryMorph(hit, out var g), g));
	}

	[Test]
	public void GetSheetFalse()
	{
		var morph = new GameObject("rayCast").AddComponent<RayCastHitToCharacterSheetMB>();
		var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		Physics.Raycast(new Ray(Vector3.up, Vector3.down), out RaycastHit hit);

		Assert.False(morph.TryMorph(hit, out _));
	}
}
