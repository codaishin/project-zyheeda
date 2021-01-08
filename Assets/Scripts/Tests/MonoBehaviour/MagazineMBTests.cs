using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MagazineMBTests : TestCollection
{
	private class MockComponent : MonoBehaviour {}

	[Test]
	public void InstantiateProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab");

		magazine.prefab = prefab;

		Assert.NotNull(magazine.GetOrMakeProjectile());
	}

	[Test]
	public void InstantiatePrefab()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.prefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();

		CollectionAssert.AreEqual(
			new bool[] { true, true },
			new bool[] {
				projectile.gameObject != prefab.gameObject,
				projectile.TryGetComponent(out MockComponent _),
			}
		);
	}
}
