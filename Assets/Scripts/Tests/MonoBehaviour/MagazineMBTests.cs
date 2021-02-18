using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MagazineMBTests : TestCollection
{
	private class MockComponent : MonoBehaviour {}

	[Test]
	public void InstantiatePrefab()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();

		CollectionAssert.AreEqual(
			new bool[] { true, true },
			new bool[] {
				projectile.Value != prefab.gameObject,
				projectile.Value.TryGetComponent(out MockComponent _),
			}
		);
	}

	[Test]
	public void StoreInMagazineTransform()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();
		projectile.Dispose();

		Assert.AreSame(magazine.transform, projectile.Value.transform.parent);
	}

	[Test]
	public void StoreInMagazineInactive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();
		projectile.Dispose();

		Assert.False(projectile.Value.activeSelf);
	}

	[UnityTest]
	public IEnumerator OnlyOneProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectileA.Dispose();

		yield return new WaitForEndOfFrame();

		var projectileB = magazine.GetOrMakeProjectile();

		Assert.AreSame(projectileA.Value, projectileB.Value);
	}

	[Test]
	public void TwoProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.GetOrMakeProjectile();
		var projectileB = magazine.GetOrMakeProjectile();

		Assert.AreNotSame(projectileA.Value, projectileB.Value);
	}

	[UnityTest]
	public IEnumerator ProjectileActive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Dispose();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile();

		Assert.True(projectile.Value.activeSelf);
	}

	[UnityTest]
	public IEnumerator ProjectileNoParent()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Dispose();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile();

		Assert.Null(projectile.Value.transform.parent);
	}

	[Test]
	public void Projectiles()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab");

		magazine.projectilePrefab = prefab;

		var projectiles = new GameObject[] {
			magazine.GetOrMakeProjectile().Value,
			magazine.GetOrMakeProjectile().Value,
		};

		CollectionAssert.AreEqual(projectiles, magazine.Projectiles);
	}
}
