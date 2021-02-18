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

		var projectile = magazine.UseProjectile();

		CollectionAssert.AreEqual(
			new bool[] { true, true },
			new bool[] {
				projectile.gameObject != prefab.gameObject,
				projectile.gameObject.TryGetComponent(out MockComponent _),
			}
		);
	}

	[Test]
	public void StoreInMagazineTransform()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.UseProjectile();
		projectile.Dispose();

		Assert.AreSame(magazine.transform, projectile.gameObject.transform.parent);
	}

	[Test]
	public void StoreInMagazineInactive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.UseProjectile();
		projectile.Dispose();

		Assert.False(projectile.gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator OnlyOneProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.UseProjectile();

		yield return new WaitForEndOfFrame();

		projectileA.Dispose();

		yield return new WaitForEndOfFrame();

		var projectileB = magazine.UseProjectile();

		Assert.AreSame(projectileA.gameObject, projectileB.gameObject);
	}

	[Test]
	public void TwoProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.UseProjectile();
		var projectileB = magazine.UseProjectile();

		Assert.AreNotSame(projectileA.gameObject, projectileB.gameObject);
	}

	[UnityTest]
	public IEnumerator ProjectileActive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.UseProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Dispose();

		yield return new WaitForEndOfFrame();

		magazine.UseProjectile();

		Assert.True(projectile.gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator ProjectileNoParent()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.UseProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Dispose();

		yield return new WaitForEndOfFrame();

		magazine.UseProjectile();

		Assert.Null(projectile.gameObject.transform.parent);
	}

	[Test]
	public void Projectiles()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab");

		magazine.projectilePrefab = prefab;

		var projectiles = new GameObject[] {
			magazine.UseProjectile().gameObject,
			magazine.UseProjectile().gameObject,
		};

		CollectionAssert.AreEqual(projectiles, magazine.Projectiles);
	}
}
