using System.Collections.Generic;
using UnityEngine;

public static class ProjectileRoutine
{
	public static
	IEnumerator<WaitForFixedUpdate> Create(Transform from, Transform to, float deltaPerSecond)
	{
		ProjectileLauncherMB launcher = from.gameObject.RequireComponent<ProjectileLauncherMB>();
		ProjectileMB projectile = launcher.Magazine.GetOrMakeProjectile();
		projectile.transform.position = launcher.spawnProjectilesAt.position;
		while (projectile.transform.position != to.position) {
			projectile.transform.position = Vector3.MoveTowards(
				projectile.transform.position,
				to.position,
				deltaPerSecond * Time.fixedDeltaTime
			);
			projectile.transform.LookAt(to);
			yield return new WaitForFixedUpdate();
		}
		projectile.Store();
	}
}
