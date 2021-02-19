using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class CastProjectileTests : TestCollection
{
	[Test]
	public void MoveProjectileToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("launcher").AddComponent<MagazineMB>();
		var cast = new CastProjectile{ magazine = magazine, projectileSpawn = spawn };
		cast.projectileSpeed = 1;
		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		var iterator = cast.Apply(target);

		iterator.MoveNext();

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime,
			magazine.Projectiles.First().transform.position
		);
	}

	[Test]
	public void ProjectileSpawnPosition()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("launcher").AddComponent<MagazineMB>();
		var cast = new CastProjectile{ magazine = magazine, projectileSpawn = spawn };
		cast.projectileSpeed = 1;
		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.back;
		spawn.transform.position = Vector3.back;

		cast.Apply(target).MoveNext();

		Assert.AreEqual(Vector3.back, magazine.Projectiles.First().transform.position);
	}

	[Test]
	public void MoveProjectileFullyToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("launcher").AddComponent<MagazineMB>();
		var cast = new CastProjectile{ magazine = magazine, projectileSpawn = spawn };
		cast.projectileSpeed = 0.5f;
		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		var iterator = cast.Apply(target);

		while (iterator.MoveNext());

		Tools.AssertEqual(
			target.transform.position,
			magazine.Projectiles.First().transform.position
		);
	}

	[Test]
	public void StoreProjectileWhenTargetReached()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("launcher").AddComponent<MagazineMB>();
		var cast = new CastProjectile{ magazine = magazine, projectileSpawn = spawn };
		cast.projectileSpeed = 0.5f;
		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		var iterator = cast.Apply(target);

		while (iterator.MoveNext());

		var projectile = magazine.Projectiles.First();

		Assert.AreEqual(
			(true, true),
			(!projectile.activeSelf, projectile.transform.parent == magazine.transform)
		);
	}

	[Test]
	public void ProjectileLookAtTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("launcher").AddComponent<MagazineMB>();
		var cast = new CastProjectile{ magazine = magazine, projectileSpawn = spawn };
		cast.projectileSpeed = 1;
		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.up;
		var iterator = cast.Apply(target);

		iterator.MoveNext();

		var expected = Quaternion.LookRotation(Vector3.up);

		Tools.AssertEqual(
			expected.eulerAngles,
			magazine.Projectiles.First().transform.rotation.eulerAngles
		);
	}
}
