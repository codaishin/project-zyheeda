using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CastProjectile : ICast, ISetGameObject, IGetGameObject
{
	private ProjectileLauncherMB launcher;
	public float deltaPerSecond;

	public GameObject gameObject {
		get => this.launcher.gameObject;
		set => this.launcher = value.RequireComponent<ProjectileLauncherMB>();
	}

	public IEnumerator<WaitForFixedUpdate> Apply(GameObject target)
	{
		ProjectileMB projectile = this.launcher.Magazine.GetOrMakeProjectile();
		projectile.transform.position = launcher.spawnProjectilesAt.position;
		while (projectile.transform.position != target.transform.position) {
			projectile.transform.position = Vector3.MoveTowards(
				projectile.transform.position,
				target.transform.position,
				deltaPerSecond * Time.fixedDeltaTime
			);
			projectile.transform.LookAt(target.transform);
			yield return new WaitForFixedUpdate();
		}
		projectile.Store();
		yield break;
	}
}
