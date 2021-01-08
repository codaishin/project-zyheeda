using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectileMBTests : TestCollection
{
	[Test]
	public void StoreInMagazineTransform()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var projectile = new GameObject("projectile").AddComponent<ProjectileMB>();

		projectile.Magazine = magazine;

		projectile.Store();

		Assert.AreSame(magazine.transform, projectile.transform.parent);
	}

	[Test]
	public void StoreInMagazineInactive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var projectile = new GameObject("projectile").AddComponent<ProjectileMB>();

		projectile.Magazine = magazine;

		projectile.Store();

		Assert.False(projectile.gameObject.activeSelf);
	}
}
