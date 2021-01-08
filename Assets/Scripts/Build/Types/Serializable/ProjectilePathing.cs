using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ProjectilePathing : IProjectileManager
{
	public Transform spawnPoint;
	public BaseMagazineMB magazine;
	public float deltaPerSecond;

	private Vector3 Interpolate(in Transform current, in Transform target) =>
 		Vector3.MoveTowards(
			current.position,
			target.position,
			this.deltaPerSecond * Time.fixedDeltaTime
		);

	public IEnumerator<WaitForFixedUpdate> ProjectileRoutineTo(Transform target)
	{
		Transform projectile = this.magazine.GetOrMakeProjectile().transform;
		projectile.position = this.spawnPoint.position;
		while (projectile.position != target.position) {
			projectile.position = this.Interpolate(projectile, target);
			projectile.LookAt(target);
			yield return new WaitForFixedUpdate();
		}
		projectile.gameObject.SetActive(false);
	}
}
