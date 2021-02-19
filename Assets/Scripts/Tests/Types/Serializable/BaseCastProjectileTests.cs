using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class BaseCastProjectileTests : TestCollection
{
	private class MockMagazine : IMagazine
	{
		public OnDisposeFunc<GameObject> onDispose = (in GameObject o) => {};

		public GameObject Projectile { get; } = new GameObject("projectile");

		public Disposable<GameObject> GetOrMakeProjectile()
		{
			return this.Projectile.AsDisposable(this.onDispose);
		}
	}

	private class MockCastProjectile : BaseCastProjectile<MockMagazine> {}

	[Test]
	public void MoveProjectileToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.projectileSpeed = 1;
		target.transform.position = Vector3.right;
		var iterator = cast.Apply(target);

		iterator.MoveNext();

		Tools.AssertEqual(
			Vector3.right * Time.fixedDeltaTime,
			cast.magazine.Projectile.transform.position
		);
	}

	[Test]
	public void ProjectileSpawnPosition()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.projectileSpeed = 1;
		target.transform.position = Vector3.back;
		spawn.transform.position = Vector3.back;

		cast.Apply(target).MoveNext();

		Assert.AreEqual(Vector3.back, cast.magazine.Projectile.transform.position);
	}

	[Test]
	public void MoveProjectileFullyToTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.projectileSpeed = 0.5f;
		target.transform.position = Vector3.right;
		var iterator = cast.Apply(target);

		while (iterator.MoveNext());

		Tools.AssertEqual(
			target.transform.position,
			cast.magazine.Projectile.transform.position
		);
	}

	[Test]
	public void DisposeProjectileWhenTargetReached()
	{
		var disposed = null as GameObject;
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.projectileSpeed = 0.5f;
		cast.magazine.onDispose = (in GameObject o) => disposed = o;
		target.transform.position = Vector3.right;
		var iterator = cast.Apply(target);

		while (iterator.MoveNext());

		Assert.AreSame(cast.magazine.Projectile, disposed);
	}

	[Test]
	public void ProjectileLookAtTarget()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.projectileSpeed = 1;
		target.transform.position = Vector3.up;
		var iterator = cast.Apply(target);

		iterator.MoveNext();

		var expected = Quaternion.LookRotation(Vector3.up);

		Tools.AssertEqual(
			expected.eulerAngles,
			cast.magazine.Projectile.transform.rotation.eulerAngles
		);
	}
}
