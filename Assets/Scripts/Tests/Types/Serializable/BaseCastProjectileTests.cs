using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseCastProjectileTests : TestCollection
{
	private class MockSheet {}

	private class MockApproach : IApproach<MockSheet>
	{
		public
		Func<Transform, MockSheet, float, IEnumerator<WaitForFixedUpdate>> apply =
			MockApproach.DefaultApproach;

		public
		IEnumerator<WaitForFixedUpdate> Apply(Transform transform, MockSheet target, float speed) =>
			this.apply(transform, target, speed);

		private static
		IEnumerator<WaitForFixedUpdate> DefaultApproach(Transform _, MockSheet __, float ___)
		{
			yield break;
		}
	}

	private class MockMagazine : IMagazine
	{
		public Action<GameObject> onDispose = _ => {};

		public GameObject Projectile { get; } = new GameObject("projectile");

		public Disposable<GameObject> GetOrMakeProjectile()
		{
			return this.Projectile.AsDisposable(this.onDispose);
		}
	}

	private class MockCastProjectile : BaseCastProjectile<MockMagazine, MockApproach, MockSheet> { }

	[Test]
	public void ApproachArgs()
	{
		var called = default((Transform, MockSheet, float));
		var spawn = new GameObject("spawn");
		var target = new MockSheet();
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.projectileSpeed = 1;

		IEnumerator<WaitForFixedUpdate> approach(Transform projectile, MockSheet target, float speed) {
			called = (projectile, target, speed);
			yield break;
		}
		cast.approach.apply = approach;

		cast.Apply(target).MoveNext();

		Assert.AreEqual((cast.magazine.Projectile.transform, target, 1f), called);
	}

	[Test]
	public void ProjectileSpawnPosition()
	{
		var spawn = new GameObject("spawn");
		var target = new MockSheet();
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		spawn.transform.position = Vector3.back;

		cast.Apply(target).MoveNext();

		Assert.AreEqual(Vector3.back, cast.magazine.Projectile.transform.position);
	}

	[Test]
	public void DisposeProjectileWhenRoutineDone()
	{
		var disposed = null as GameObject;
		var spawn = new GameObject("spawn");
		var target = new MockSheet();
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.magazine.onDispose =o => disposed = o;

		cast.Apply(target).MoveNext();

		Assert.AreSame(cast.magazine.Projectile, disposed);
	}

		[Test]
	public void DontDisposeProjectileWhenRoutineNotDone()
	{
		var disposed = false;
		var spawn = new GameObject("spawn");
		var target = new MockSheet();
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.magazine.onDispose = _ => disposed = true;

		IEnumerator<WaitForFixedUpdate> approach(Transform _, MockSheet __, float ___) {
			yield return new WaitForFixedUpdate();
		}
		cast.approach.apply = approach;

		cast.Apply(target).MoveNext();

		Assert.False(disposed);
	}
}
