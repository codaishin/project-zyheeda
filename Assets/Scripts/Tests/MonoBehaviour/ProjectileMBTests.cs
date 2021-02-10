using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectileMBTests : TestCollection
{
	[Test]
	public void Init()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var projectile = ProjectileMB.Init<ProjectileMB>(new GameObject("projectile"), magazine);

		Assert.AreSame(magazine, projectile.Magazine);
	}

	[Test]
	public void StoreInMagazineTransform()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var projectile = new GameObject("projectile").AddComponent<ProjectileMB>(magazine);

		projectile.Store();

		Assert.AreSame(magazine.transform, projectile.transform.parent);
	}

	[Test]
	public void StoreInMagazineInactive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var projectile = new GameObject("projectile").AddComponent<ProjectileMB>(magazine);

		projectile.Store();

		Assert.False(projectile.gameObject.activeSelf);
	}
}
