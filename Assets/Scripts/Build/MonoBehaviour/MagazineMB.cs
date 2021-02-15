using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MagazineMB : BaseMagazineMB
{
	private List<ProjectileMB> projectiles = new List<ProjectileMB>();

	public GameObject projectilePrefab;

	public IEnumerable<ProjectileMB> Projectiles => this.projectiles;

	private ProjectileMB MakeProjectile()
	{
		ProjectileMB projectile = GameObject
			.Instantiate(this.projectilePrefab)
			.AddComponent<ProjectileMB>(magazine: this);
		this.projectiles.Add(projectile);
		return projectile;
	}

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
