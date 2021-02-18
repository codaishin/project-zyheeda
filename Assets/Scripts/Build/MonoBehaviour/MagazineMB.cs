using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MagazineMB : MonoBehaviour
{
	private List<GameObject> projectiles = new List<GameObject>();

	public GameObject projectilePrefab;

	public IEnumerable<GameObject> Projectiles => this.projectiles;

	private GameObject MakeProjectile()
	{
		GameObject projectile = GameObject.Instantiate(this.projectilePrefab);
		this.projectiles.Add(projectile);
		return projectile;
	}

	private bool GetProjectile(out GameObject projectile) =>
		projectile = this.projectiles
			.Where(p => !p.gameObject.activeSelf)
			.FirstOrDefault();

	private void StoreProjectile(in GameObject projectile)
	{
		projectile.gameObject.SetActive(false);
		projectile.transform.SetParent(this.transform);
	}

	public GameObject GetOrMakeProjectile(out Action storeHandle)
	{
		if (!this.GetProjectile(out GameObject projectile)) {
			projectile = this.MakeProjectile();
		}
		projectile.gameObject.SetActive(true);
		projectile.transform.SetParent(null);
		storeHandle = () => this.StoreProjectile(projectile);
		return projectile;
	}
}
