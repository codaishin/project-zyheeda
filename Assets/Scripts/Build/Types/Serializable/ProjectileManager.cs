using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileManager : IProjectileManager
{
	private List<Transform> projectiles = new List<Transform>();

	public Transform spawnPoint;
	public GameObject prefab;
	public float deltaPerSecond;

	public IEnumerable<Transform> Projectiles => this.projectiles;

	private Vector3 Interpolate(in Transform current, in Transform target) =>
 		Vector3.MoveTowards(
			current.position,
			target.position,
			this.deltaPerSecond * Time.fixedDeltaTime
		);

	private bool FirstInactiveProjectile(out Transform projectile)
	{
		projectile = this.projectiles
			.Where(p => !p.gameObject.activeSelf)
			.FirstOrDefault();
		return projectile;
	}

	private Transform GetOrMakeProjectile()
	{
		if (!this.FirstInactiveProjectile(out Transform projectile)) {
			projectile = GameObject.Instantiate(this.prefab).transform;
			this.projectiles.Add(projectile);
		}
		projectile.position = this.spawnPoint.position;
		return projectile;
	}

	private void Activate(in Transform projectile)
	{
		projectile.gameObject.SetActive(true);
		projectile.parent = null;
	}

	private void Deactivate(in Transform projectile)
	{
		projectile.gameObject.SetActive(false);
		projectile.parent = this.spawnPoint;
	}

	public IEnumerator<WaitForFixedUpdate> ProjectileRoutineTo(Transform target)
	{
		Transform projectile = this.GetOrMakeProjectile();
		this.Activate(projectile);
		while (projectile.position != target.position) {
			projectile.position = this.Interpolate(projectile, target);
			projectile.LookAt(target);
			yield return new WaitForFixedUpdate();
		}
		this.Deactivate(projectile);
	}
}
