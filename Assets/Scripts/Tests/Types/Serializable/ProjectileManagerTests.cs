using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectileManagerTests : TestCollection
{
	private class MockPrefabMB : MonoBehaviour {}

	[Test]
	public void ProjectileInstantiate()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.ProjectileRoutineTo(target.transform).MoveNext();

		Assert.AreEqual(1, manager.Projectiles.Count());
	}

	[Test]
	public void ProjectileInstantiatePrefab()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab").AddComponent<MockPrefabMB>();
		var manager = new ProjectileManager();

		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab.gameObject;
		manager.ProjectileRoutineTo(target.transform).MoveNext();

		Assert.True(manager.Projectiles.First().TryGetComponent(out MockPrefabMB _));
	}

	[Test]
	public void MoveProjectileToTarget()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 1;
		var iterator = manager.ProjectileRoutineTo(target.transform);

		iterator.MoveNext();

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime,
			manager.Projectiles.First().transform.position
		);
	}

	[Test]
	public void MoveProjectileFullyToTarget()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);

		while (iterator.MoveNext());

		Tools.AssertEqual(
			target.transform.position,
			manager.Projectiles.First().transform.position
		);
	}

	[Test]
	public void DisableProjectileWhenTargetReached()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);

		while (iterator.MoveNext());

		Assert.False(manager.Projectiles.First().gameObject.activeSelf);
	}

	[Test]
	public void AttachProjectileWhenTargetReached()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);

		while (iterator.MoveNext());

		Assert.AreSame(spawn.transform, manager.Projectiles.First().parent);
	}

	[Test]
	public void ProjectileNotDisabledInFlight()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);
		var states = new List<bool>();

		while (iterator.MoveNext()) {
			states.Add(manager.Projectiles.First().gameObject.activeSelf);
		}

		Assert.True(states.All(s => s));
	}

	[Test]
	public void ProjectileDetachedInFlight()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);
		var states = new List<Transform>();

		while (iterator.MoveNext()) {
			states.Add(manager.Projectiles.First().parent);
		}

		Assert.True(states.All(s => s == null));
	}

	[Test]
	public void ReuseProjectile()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);
		var states = new List<bool>();

		while (iterator.MoveNext());

		manager.ProjectileRoutineTo(target.transform).MoveNext();

		Assert.AreEqual(1, manager.Projectiles.Count());
	}

	[Test]
	public void ReusedProjectileActive()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);
		var states = new List<bool>();

		while (iterator.MoveNext());

		target.transform.position += Vector3.one;
		manager.ProjectileRoutineTo(target.transform).MoveNext();

		Assert.True(manager.Projectiles.First().gameObject.activeSelf);
	}

	[Test]
	public void ReusedProjectileAtSpawnPosition()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		var iterator = manager.ProjectileRoutineTo(target.transform);
		var states = new List<bool>();

		while (iterator.MoveNext());

		spawn.transform.position = Vector3.left;
		target.transform.position = Vector3.left;
		manager.ProjectileRoutineTo(target.transform).MoveNext();

		Assert.AreEqual(Vector3.left, manager.Projectiles.First().position);
	}

	[Test]
	public void UseNewProjectile()
	{
		var spawn = new GameObject("self");
		var target = new GameObject("target");
		var prefab = new GameObject("prefab");
		var manager = new ProjectileManager();

		target.transform.position = Vector3.right;
		manager.spawnPoint = spawn.transform;
		manager.prefab = prefab;
		manager.deltaPerSecond = 0.5f;
		manager.ProjectileRoutineTo(target.transform).MoveNext();
		manager.ProjectileRoutineTo(target.transform).MoveNext();

		Assert.AreEqual(2, manager.Projectiles.Count());
	}
}
