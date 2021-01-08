using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MagazineMB : MonoBehaviour
{
	public ProjectileMB projectilePrefab;

	private ProjectileMB MakeProjectile()
	{
		ProjectileMB projectile = GameObject.Instantiate(this.projectilePrefab);
		projectile.Magazine = this;
		return projectile;
	}

	public ProjectileMB GetOrMakeProjectile()
	{
		return this.MakeProjectile();
	}
}
