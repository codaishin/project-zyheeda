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
		projectile.SetActive(false);
		projectile.transform.SetParent(this.transform);
	}

	public Disposable<GameObject> GetOrMakeProjectile()
	{
		if (!this.GetProjectile(out GameObject projectile)) {
			projectile = this.MakeProjectile();
		}
		projectile.SetActive(true);
		projectile.transform.SetParent(null);
		return projectile.AsDisposable(this.StoreProjectile);
	}
}
