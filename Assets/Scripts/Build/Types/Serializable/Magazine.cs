using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Magazine : IMagazine
{
	private List<GameObject> projectiles = new List<GameObject>();
	public GameObject? projectilePrefab;
	public Transform? projectileStorage;

	private GameObject MakeProjectile() {
		if (this.projectilePrefab == null) throw this.NullError();
		GameObject projectile = GameObject.Instantiate(this.projectilePrefab);
		this.projectiles.Add(projectile);
		return projectile;
	}

	private bool GetProjectile(out GameObject projectile) =>
		projectile = this.projectiles
			.Where(p => !p.gameObject.activeSelf)
			.FirstOrDefault();

	private void StoreProjectile(GameObject projectile) {
		projectile.SetActive(false);
		projectile.transform.SetParent(this.projectileStorage);
	}

	public Disposable<GameObject> GetOrMakeProjectile() {
		if (!this.GetProjectile(out GameObject projectile)) {
			projectile = this.MakeProjectile();
		}
		projectile.SetActive(true);
		projectile.transform.SetParent(null);
		return projectile.AsDisposable(this.StoreProjectile);
	}
}
