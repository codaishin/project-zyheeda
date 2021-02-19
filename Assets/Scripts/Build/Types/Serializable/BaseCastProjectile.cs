using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseCastProjectile<TMagazine> : ICast
	where TMagazine: IMagazine, new()
{
	public Transform projectileSpawn;
	public TMagazine magazine = new TMagazine();
	public float projectileSpeed;

	public IEnumerator<WaitForFixedUpdate> Apply(GameObject target)
	{
		using (this.magazine.GetOrMakeProjectile().Use(out GameObject projectile)) {
			projectile.transform.position = this.projectileSpawn.position;
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
