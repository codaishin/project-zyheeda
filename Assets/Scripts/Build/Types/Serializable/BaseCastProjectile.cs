using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BaseCastProjectile<TMagazine, TApproach> : ICast
	where TMagazine: IMagazine, new()
	where TApproach: IApproach<GameObject>, new()
{
	public TMagazine magazine = new TMagazine();
	public TApproach approach = new TApproach();
	public Transform projectileSpawn;
	public float projectileSpeed;

	private IEnumerable<WaitForFixedUpdate> Apply(Transform projectile, GameObject target)
	{
		projectile.transform.position = this.projectileSpawn.position;
		using (IEnumerator<WaitForFixedUpdate> it = this.approach.Apply(projectile, target, this.projectileSpeed)) {
			while (it.MoveNext()) {
				yield return it.Current;
			}
		}
	}

	public IEnumerator<WaitForFixedUpdate> Apply(GameObject target)
	{
		using (Disposable<GameObject> projectile = this.magazine.GetOrMakeProjectile()) {
			foreach (WaitForFixedUpdate yield in this.Apply(projectile.Value.transform, target)) {
				yield return yield;
			}
		}
	}
}
