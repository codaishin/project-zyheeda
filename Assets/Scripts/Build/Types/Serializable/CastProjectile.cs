using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CastProjectile : ICast, ISetGameObject, IGetGameObject
{
	private ProjectileLauncherMB launcher;
	public float projectileSpeed;

	public GameObject gameObject {
		get => this.launcher.gameObject;
		set => this.launcher = value.RequireComponent<ProjectileLauncherMB>();
	}

	public IEnumerator<WaitForFixedUpdate> Apply(GameObject target)
	{
		using (this.launcher.Magazine.GetOrMakeProjectile().Use(out GameObject projectile)) {
			projectile.transform.position = launcher.spawnProjectilesAt.position;
			while (projectile.transform.position != target.transform.position) {
				projectile.transform.position = Vector3.MoveTowards(
					projectile.transform.position,
					target.transform.position,
					this.projectileSpeed * Time.fixedDeltaTime
				);
				projectile.transform.LookAt(target.transform);
				yield return new WaitForFixedUpdate();
			}
		}
	}
}
