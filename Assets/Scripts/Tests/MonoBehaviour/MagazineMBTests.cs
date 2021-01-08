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
		var prefab = new GameObject("prefab").AddComponent<ProjectileMB>();

		magazine.projectilePrefab = prefab;

		Assert.NotNull(magazine.GetOrMakeProjectile());
	}

	[Test]
	public void InstantiatePrefab()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab")
			.AddComponent<MockComponent>().gameObject
			.AddComponent<ProjectileMB>();

		magazine.projectilePrefab = prefab;

		var projectile = magazine.GetOrMakeProjectile();

		CollectionAssert.AreEqual(
			new bool[] { true, true },
			new bool[] {
				projectile.gameObject != prefab.gameObject,
				projectile.TryGetComponent(out MockComponent _),
			}
		);
	}

	[Test]
	public void SetProjectileMagazine()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<ProjectileMB>();

		magazine.projectilePrefab = prefab;

		var projectile = magazine.GetOrMakeProjectile();
		Assert.AreSame(magazine, projectile.Magazine);
	}
}
