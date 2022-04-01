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
		projectile.Apply(target);
	}

	public void Release(Transform target) { }
}

public class MagazineMB : BaseMagazineMB<ProjectileMB> { }
