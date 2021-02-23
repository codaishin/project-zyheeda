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

	protected abstract Movement.ApproachFunc<GameObject> Approach { get; }

	private IEnumerable<WaitForFixedUpdate> Apply(Transform projectile, GameObject target)
	{
		projectile.transform.position = this.projectileSpawn.position;
		using (IEnumerator<WaitForFixedUpdate> iterator = this.Approach(projectile, target, this.projectileSpeed)) {
			while (iterator.MoveNext()) {
				yield return iterator.Current;
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
