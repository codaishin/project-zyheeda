using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MagazineMB : BaseMagazineMB
{
	private List<ProjectileMB> projectiles = new List<ProjectileMB>();

	public ProjectileMB projectilePrefab;

	private ProjectileMB InstantiateCoupled(in ProjectileMB projectilePrefab)
	{
		ProjectileMB projectile = GameObject.Instantiate(projectilePrefab);
		projectile.Magazine = this;
		this.projectiles.Add(projectile);
		return projectile;
	}

	private ProjectileMB MakeProjectile() =>
		this.InstantiateCoupled(this.projectilePrefab);

	private bool GetProjectile(out ProjectileMB projectile) =>
		projectile = this.projectiles
			.Where(p => !p.gameObject.activeSelf)
			.FirstOrDefault();

	public override ProjectileMB GetOrMakeProjectile()
	{
		if (!this.GetProjectile(out ProjectileMB projectile)) {
			projectile = this.MakeProjectile();
		}
		projectile.gameObject.SetActive(true);
		projectile.transform.SetParent(null);
		return projectile;
	}
}
