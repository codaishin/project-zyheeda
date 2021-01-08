using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MagazineMB : BaseMagazineMB
{
	private List<ProjectileMB> projectiles = new List<ProjectileMB>();

	public ProjectileMB projectilePrefab;

	private ProjectileMB CoupleWith(in ProjectileMB projectile)
	{
		projectile.Magazine = this;
		this.projectiles.Add(projectile);
		return projectile;
	}

	private ProjectileMB MakeProjectile() =>
		this.CoupleWith(GameObject.Instantiate(this.projectilePrefab));

	private bool GetProjectile(out ProjectileMB projectile) =>
		projectile = this.projectiles
			.Where(p => !p.enabled)
			.FirstOrDefault();

	public override ProjectileMB GetOrMakeProjectile() =>
		this.GetProjectile(out ProjectileMB projectile)
			? projectile
			: this.MakeProjectile();
}
