using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMagazineMB<TProjectile> :
	MonoBehaviour,
	IApplicable<Transform>
	where TProjectile :
		MonoBehaviour,
		IApplicable<Transform>
{
	public TProjectile? prefab;
	public Transform? spawnPoint;

	public void Apply(Transform target) {
		TProjectile projectile = GameObject.Instantiate(
			this.prefab,
			this.spawnPoint!.position,
			Quaternion.identity
		)!;
		var apply = BaseMagazineMB<TProjectile>.DelayedApply(projectile, target);
		projectile.StartCoroutine(apply);
	}

	private static IEnumerator<WaitForEndOfFrame> DelayedApply(
		TProjectile projectile,
		Transform target
	) {
		yield return new WaitForEndOfFrame();
		projectile.Apply(target);
	}
}

public class MagazineMB : BaseMagazineMB<ProjectileMB> { }
