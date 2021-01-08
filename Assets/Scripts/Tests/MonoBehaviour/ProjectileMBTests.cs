using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectileMBTests : TestCollection
{
	[UnityTest]
	public IEnumerator StoreAsMagazineChildWhenInactive()
	{
		var magazine = new GameObject("magazine").AddComponent<MagazineMB>();
		var projectile = new GameObject("projectile").AddComponent<ProjectileMB>();

		projectile.Magazine = magazine;

		yield return new WaitForEndOfFrame();

		projectile.enabled = false;

		yield return new WaitForEndOfFrame();

		Assert.AreSame(magazine.transform, projectile.transform.parent);
	}
}
