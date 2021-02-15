using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class ProjectileRoutineTests : TestCollection
{
	[Test]
	public void MoveProjectileToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var launcher = new GameObject("launcher").AddComponent<ProjectileLauncherMB>();
		launcher.spawnProjectilesAt = spawn.transform;

		launcher.Magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		var iterator = ProjectileRoutine.Create(launcher.transform, target.transform, 1);

		iterator.MoveNext();

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime,
			launcher.Magazine.Projectiles.First().transform.position
		);
	}

	[Test]
	public void ProjectileSpawnPosition()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var launcher = new GameObject("launcher").AddComponent<ProjectileLauncherMB>();
		launcher.spawnProjectilesAt = spawn.transform;

		launcher.Magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.back;
		spawn.transform.position = Vector3.back;

		ProjectileRoutine.Create(launcher.transform, target.transform, 1).MoveNext();

		Assert.AreEqual(Vector3.back, launcher.Magazine.Projectiles.First().transform.position);
	}

	[Test]
	public void MoveProjectileFullyToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var launcher = new GameObject("launcher").AddComponent<ProjectileLauncherMB>();
		launcher.spawnProjectilesAt = spawn.transform;

		launcher.Magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		var iterator = ProjectileRoutine.Create(launcher.transform, target.transform, 0.5f);

		while (iterator.MoveNext());

		Tools.AssertEqual(
			target.transform.position,
			launcher.Magazine.Projectiles.First().transform.position
		);
	}

	[Test]
	public void StoreProjectileWhenTargetReached()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var launcher = new GameObject("launcher").AddComponent<ProjectileLauncherMB>();
		launcher.spawnProjectilesAt = spawn.transform;

		launcher.Magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.right;
		var iterator = ProjectileRoutine.Create(launcher.transform, target.transform, 0.5f);

		while (iterator.MoveNext());

		var projectile = launcher.Magazine.Projectiles.First();

		Assert.AreEqual(
			(true, true),
			(!projectile.isActiveAndEnabled, projectile.transform.parent == launcher.Magazine.transform)
		);
	}

	[Test]
	public void ProjectileLookAtTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var launcher = new GameObject("launcher").AddComponent<ProjectileLauncherMB>();
		launcher.spawnProjectilesAt = spawn.transform;

		launcher.Magazine.projectilePrefab = new GameObject("projectile");
		target.transform.position = Vector3.up;
		var iterator = ProjectileRoutine.Create(launcher.transform, target.transform, 1);

		iterator.MoveNext();

		var expected = Quaternion.LookRotation(Vector3.up);

		Tools.AssertEqual(
			expected.eulerAngles,
			launcher.Magazine.Projectiles.First().transform.rotation.eulerAngles
		);
	}
}
