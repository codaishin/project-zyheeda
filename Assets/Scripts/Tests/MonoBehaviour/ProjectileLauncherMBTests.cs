using NUnit.Framework;
using UnityEngine;

public class ProjectileLauncherMBTests : TestCollection
{
	[Test]
	public void RequiresMagazine()
	{
		var launcher = new GameObject("launcher").AddComponent<ProjectileLauncherMB>();

		Assert.True(launcher.TryGetComponent(out MagazineMB _));
	}

	[Test]
	public void MagazineProperty()
	{
		var launcher = new GameObject("launcher").AddComponent<ProjectileLauncherMB>();

		Assert.AreSame(launcher.GetComponent<MagazineMB>(), launcher.Magazine);
	}
}
