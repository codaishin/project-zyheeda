using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectilePathingTests : TestCollection
{
	[Test]
	public void MoveProjectileToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var pathing = new ProjectilePathing();

		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 1;
		var iterator = pathing.ProjectileRoutine(null, target.transform);

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
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var pathing = new ProjectilePathing();

		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.back;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		spawn.transform.position = Vector3.back;

		pathing.ProjectileRoutine(null, target.transform).MoveNext();

		Assert.AreEqual(Vector3.back, magazine.Projectiles.First().transform.position);
	}

	[Test]
	public void MoveProjectileFullyToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var pathing = new ProjectilePathing();

		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 0.5f;
		var iterator = pathing.ProjectileRoutine(null, target.transform);

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
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var pathing = new ProjectilePathing();

		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 0.5f;
		var iterator = pathing.ProjectileRoutine(null, target.transform);

		while (iterator.MoveNext());

		var projectile = magazine.Projectiles.First();

		Assert.AreEqual(
			(true, true),
			(!projectile.isActiveAndEnabled, projectile.transform.parent == magazine.transform)
		);
	}

	[Test]
	public void ProjectileLookAtTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var pathing = new ProjectilePathing();

		magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.up;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 1;
		var iterator = pathing.ProjectileRoutine(null, target.transform);

		iterator.MoveNext();

		var expected = Quaternion.LookRotation(Vector3.up);

		Tools.AssertEqual(
			expected.eulerAngles,
			magazine.Projectiles.First().transform.rotation.eulerAngles
		);
	}
}
