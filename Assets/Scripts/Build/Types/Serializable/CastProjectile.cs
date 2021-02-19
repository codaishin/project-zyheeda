using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CastProjectile : ICast
{
	public GameObject projectileSpawn;
	public MagazineMB magazine;
	public float projectileSpeed;

	public IEnumerator<WaitForFixedUpdate> Apply(GameObject target)
	{
		using (this.magazine.GetOrMakeProjectile().Use(out GameObject projectile)) {
			projectile.transform.position = this.projectileSpawn.transform.position;
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
