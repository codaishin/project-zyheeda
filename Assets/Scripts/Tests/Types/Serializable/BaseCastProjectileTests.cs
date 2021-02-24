using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseCastProjectileTests : TestCollection
{
	private class MockApproach : IApproach<GameObject>
	{
		public
		Func<Transform, GameObject, float, IEnumerator<WaitForFixedUpdate>> approach =
			MockApproach.DefaultApproach;

		public
		IEnumerator<WaitForFixedUpdate> Apply(Transform transform, GameObject target, float speed) =>
			this.approach(transform, target, speed);

		private static
		IEnumerator<WaitForFixedUpdate> DefaultApproach(Transform _, GameObject __, float ___)
		{
			yield break;
		}
	}

	private class MockMagazine : IMagazine
	{
		public OnDisposeFunc<GameObject> onDispose = (in GameObject o) => {};

		public GameObject Projectile { get; } = new GameObject("projectile");

		public Disposable<GameObject> GetOrMakeProjectile()
		{
			return this.Projectile.AsDisposable(this.onDispose);
		}
	}

	private class MockCastProjectile : BaseCastProjectile<MockMagazine, MockApproach> { }

	[Test]
	public void ApproachArgs()
	{
		var called = default((Transform, GameObject, float));
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.projectileSpeed = 1;

		IEnumerator<WaitForFixedUpdate> approach(Transform projectile, GameObject target, float speed) {
			called = (projectile, target, speed);
			yield break;
		}
		cast.approach.approach = approach;

		cast.Apply(target).MoveNext();

		Assert.AreEqual((cast.magazine.Projectile.transform, target, 1f), called);
	}

	[Test]
	public void ProjectileSpawnPosition()
	{
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
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
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.magazine.onDispose = (in GameObject o) => disposed = o;

		cast.Apply(target).MoveNext();

		Assert.AreSame(cast.magazine.Projectile, disposed);
	}

		[Test]
	public void DontDisposeProjectileWhenRoutineNotDone()
	{
		var disposed = false;
		var spawn = new GameObject("spawn");
		var target = new GameObject("target");
		var cast = new MockCastProjectile{ projectileSpawn = spawn.transform };
		cast.magazine.onDispose = (in GameObject _) => disposed = true;

		IEnumerator<WaitForFixedUpdate> approach(Transform _, GameObject __, float ___) {
			yield return new WaitForFixedUpdate();
		}
		cast.approach.approach = approach;

		cast.Apply(target).MoveNext();

		Assert.False(disposed);
	}
}
