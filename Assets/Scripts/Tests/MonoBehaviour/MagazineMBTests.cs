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

		var projectile = magazine.GetOrMakeProjectile(out Action _);

		CollectionAssert.AreEqual(
			new bool[] { true, true },
			new bool[] {
				projectile.gameObject != prefab.gameObject,
				projectile.TryGetComponent(out MockComponent _),
			}
		);
	}

	[Test]
	public void StoreInMagazineTransform()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile(out Action store);

		store();

		Assert.AreSame(magazine.transform, projectile.transform.parent);
	}

	[Test]
	public void StoreInMagazineInactive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile(out Action store);

		store();

		Assert.False(projectile.gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator OnlyOneProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.GetOrMakeProjectile(out Action store);

		yield return new WaitForEndOfFrame();

		store();

		yield return new WaitForEndOfFrame();

		var projectileB = magazine.GetOrMakeProjectile(out Action _);

		Assert.AreSame(projectileA, projectileB);
	}

	[Test]
	public void TwoProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.GetOrMakeProjectile(out Action _);
		var projectileB = magazine.GetOrMakeProjectile(out Action _);

		Assert.AreNotSame(projectileA, projectileB);
	}

	[UnityTest]
	public IEnumerator ProjectileActive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile(out Action store);

		yield return new WaitForEndOfFrame();

		store();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile(out Action _);

		Assert.True(projectile.gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator ProjectileNoParent()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile(out Action store);

		yield return new WaitForEndOfFrame();

		store();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile(out Action _);

		Assert.Null(projectile.transform.parent);
	}

	[Test]
	public void Projectiles()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab");

		magazine.projectilePrefab = prefab;

		var projectiles = new GameObject[] {
			magazine.GetOrMakeProjectile(out Action _),
			magazine.GetOrMakeProjectile(out Action _),
		};

		CollectionAssert.AreEqual(projectiles, magazine.Projectiles);
	}
}
