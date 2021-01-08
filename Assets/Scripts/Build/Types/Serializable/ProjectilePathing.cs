using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectilePathing : IProjectilePathing
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
		ProjectileMB projectile = this.magazine.GetOrMakeProjectile();
		projectile.transform.position = this.spawnPoint.position;
		projectile.Magazine = this.magazine;
		while (projectile.transform.position != target.position) {
			projectile.transform.position = this.Interpolate(
				projectile.transform,
				target
			);
			projectile.transform.LookAt(target);
			yield return new WaitForFixedUpdate();
		}
		projectile.Store();
	}
}
