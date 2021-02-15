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

		magazine.projectilePrefab = prefab;

		Assert.NotNull(magazine.GetOrMakeProjectile());
	}

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
				projectile.gameObject != prefab.gameObject,
				projectile.TryGetComponent(out MockComponent _),
			}
		);
	}

	[Test]
	public void SetProjectileMagazine()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab");

		magazine.projectilePrefab = prefab;

		var projectile = magazine.GetOrMakeProjectile();
		Assert.AreSame(magazine, projectile.Magazine);
	}

	[UnityTest]
	public IEnumerator OnlyOneProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectileA.Store();

		yield return new WaitForEndOfFrame();

		var projectileB = magazine.GetOrMakeProjectile();

		Assert.AreSame(projectileA, projectileB);
	}

	[Test]
	public void TwoProjectile()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectileA = magazine.GetOrMakeProjectile();
		var projectileB = magazine.GetOrMakeProjectile();

		Assert.AreNotSame(projectileA, projectileB);
	}

	[UnityTest]
	public IEnumerator ProjectileActive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Store();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile();

		Assert.True(projectile.gameObject.activeSelf);
	}

	[UnityTest]
	public IEnumerator ProjectileNoParent()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();

		magazine.projectilePrefab = prefab.gameObject;

		var projectile = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Store();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile();

		Assert.Null(projectile.transform.parent);
	}

	[Test]
	public void Projectiles()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var prefab = new GameObject("prefab");

		magazine.projectilePrefab = prefab;

		var projectiles = new ProjectileMB[] {
			magazine.GetOrMakeProjectile(),
			magazine.GetOrMakeProjectile(),
		};

		CollectionAssert.AreEqual(projectiles, magazine.Projectiles);
	}
}
