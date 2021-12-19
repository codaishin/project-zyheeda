using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MagazineTests : TestCollection
{
	private class MockComponent : MonoBehaviour { }

	[Test]
	public void InstantiatePrefab() {
		var transform = new GameObject("transform").transform;
		var prefab = new GameObject("prefab").AddComponent<MockComponent>();
		var magazine = new Magazine { projectileStorage = transform, projectilePrefab = prefab.gameObject };

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
	public void StoreInMagazineTransform() {
		var transform = new GameObject("transform").transform;
		var prefab = new GameObject("prefab");
		var magazine = new Magazine { projectileStorage = transform, projectilePrefab = prefab };

		var projectile = magazine.GetOrMakeProjectile();
		projectile.Dispose();

		Assert.AreSame(magazine.projectileStorage, projectile.Value.transform.parent);
	}

	[Test]
	public void StoreInMagazineInactive() {
		var transform = new GameObject("transform").transform;
		var prefab = new GameObject("prefab");
		var magazine = new Magazine { projectileStorage = transform, projectilePrefab = prefab };

		var projectile = magazine.GetOrMakeProjectile();
		projectile.Dispose();

		Assert.False(projectile.Value.activeSelf);
	}

	[UnityTest]
	public IEnumerator OnlyOneProjectile() {
		var transform = new GameObject("transform").transform;
		var prefab = new GameObject("prefab");
		var magazine = new Magazine { projectileStorage = transform, projectilePrefab = prefab };

		var projectileA = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectileA.Dispose();

		yield return new WaitForEndOfFrame();

		var projectileB = magazine.GetOrMakeProjectile();

		Assert.AreSame(projectileA.Value, projectileB.Value);
	}

	[Test]
	public void TwoProjectile() {
		var transform = new GameObject("transform").transform;
		var prefab = new GameObject("prefab");
		var magazine = new Magazine { projectileStorage = transform, projectilePrefab = prefab };

		var projectileA = magazine.GetOrMakeProjectile();
		var projectileB = magazine.GetOrMakeProjectile();

		Assert.AreNotSame(projectileA.Value, projectileB.Value);
	}

	[UnityTest]
	public IEnumerator ProjectileActive() {
		var transform = new GameObject("transform").transform;
		var prefab = new GameObject("prefab");
		var magazine = new Magazine { projectileStorage = transform, projectilePrefab = prefab };

		var projectile = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Dispose();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile();

		Assert.True(projectile.Value.activeSelf);
	}

	[UnityTest]
	public IEnumerator ProjectileNoParent() {
		var transform = new GameObject("transform").transform;
		var prefab = new GameObject("prefab");
		var magazine = new Magazine { projectileStorage = transform, projectilePrefab = prefab };

		var projectile = magazine.GetOrMakeProjectile();

		yield return new WaitForEndOfFrame();

		projectile.Dispose();

		yield return new WaitForEndOfFrame();

		magazine.GetOrMakeProjectile();

		Assert.Null(projectile.Value.transform.parent);
	}
}
