using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseCastProjectile<TMagazine, TApproach, TSheet> : ICast<TSheet>
	where TMagazine :
		IMagazine,
		new()
	where TApproach :
		IApproach<TSheet>,
		new()
{
	public TMagazine magazine = new TMagazine();
	public TApproach approach = new TApproach();
	public Transform? projectileSpawn;
	public float projectileSpeed;

	private IEnumerable<WaitForFixedUpdate> Apply(
		Transform projectile,
		TSheet target
	) {
		if (this.projectileSpawn == null) throw this.NullError();
		projectile.transform.position = this.projectileSpawn.position;
		using (IEnumerator<WaitForFixedUpdate> it = this.approach.Apply(projectile, target, this.projectileSpeed)) {
			while (it.MoveNext()) {
				yield return it.Current;
			}
		}
	}

	public IEnumerator<WaitForFixedUpdate> Apply(TSheet target) {
		Disposable<GameObject> projectile = this.magazine.GetOrMakeProjectile();
		using (projectile) {
			Transform transform = projectile.Value.transform;
			foreach (WaitForFixedUpdate yield in this.Apply(transform, target)) {
				yield return yield;
			}
		}
	}
}
