using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectilePathingTests : TestCollection
{
	private class MockMagazineMB : BaseMagazineMB
	{
		private ProjectileMB projectile;

		public ProjectileMB Projectile => this.projectile;

		public override ProjectileMB GetOrMakeProjectile() => this.projectile;

		private void Awake()
		{
			this.projectile = new GameObject("projectile")
				.AddComponent<ProjectileMB>();
		}
	}

	[Test]
	public void MoveProjectileToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MockMagazineMB>();
		var pathing = new ProjectilePathing();

		target.transform.position = Vector3.right;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 1;
		var iterator = pathing.ProjectileRoutineTo(target.transform);

		iterator.MoveNext();

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime,
			magazine.Projectile.transform.position
		);
	}

	[Test]
	public void ProjectileSpawnPosition()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MockMagazineMB>();
		var pathing = new ProjectilePathing();

		target.transform.position = Vector3.back;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		spawn.transform.position = Vector3.back;

		pathing.ProjectileRoutineTo(target.transform).MoveNext();

		Assert.AreEqual(Vector3.back, magazine.Projectile.transform.position);
	}

	[Test]
	public void MoveProjectileFullyToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MockMagazineMB>();
		var pathing = new ProjectilePathing();

		target.transform.position = Vector3.right;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 0.5f;
		var iterator = pathing.ProjectileRoutineTo(target.transform);

		while (iterator.MoveNext());

		Tools.AssertEqual(
			target.transform.position,
			magazine.Projectile.transform.position
		);
	}

	[Test]
	public void DisableProjectileWhenTargetReached()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MockMagazineMB>();
		var pathing = new ProjectilePathing();

		target.transform.position = Vector3.right;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 0.5f;
		var iterator = pathing.ProjectileRoutineTo(target.transform);

		while (iterator.MoveNext());

		Assert.False(magazine.Projectile.gameObject.activeSelf);
	}

	[Test]
	public void ProjectileLookAtTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var magazine = new GameObject("magazine").AddComponent<MockMagazineMB>();
		var pathing = new ProjectilePathing();

		target.transform.position = Vector3.up;
		pathing.spawnPoint = spawn.transform;
		pathing.magazine = magazine;
		pathing.deltaPerSecond = 1;
		var iterator = pathing.ProjectileRoutineTo(target.transform);

		iterator.MoveNext();

		var expected = Quaternion.LookRotation(Vector3.up);

		Tools.AssertEqual(
			expected.eulerAngles,
			magazine.Projectile.transform.rotation.eulerAngles
		);
	}
}