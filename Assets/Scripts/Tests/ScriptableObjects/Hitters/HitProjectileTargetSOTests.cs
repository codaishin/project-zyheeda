using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HitProjectileTargetSOTests : TestCollection
{

	[UnityTest]
	public IEnumerator ProjectileTargetTransform() {
		var projectile = new GameObject().AddComponent<ProjectileMB>();
		var hitter = ScriptableObject.CreateInstance<HitProjectileTargetSO>();
		var target = new GameObject().transform;
		var hit = hitter.Try<Transform>(projectile.gameObject);

		projectile.target = target;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(target, hit());
	}

	[UnityTest]
	public IEnumerator ProjectileTargetPosition() {
		var projectile = new GameObject().AddComponent<ProjectileMB>();
		var hitter = ScriptableObject.CreateInstance<HitProjectileTargetSO>();
		var target = new GameObject().transform;
		var hit = hitter.TryPoint(projectile.gameObject);

		projectile.target = target;
		target.position = Vector3.one;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(Vector3.one, hit());
	}
}
