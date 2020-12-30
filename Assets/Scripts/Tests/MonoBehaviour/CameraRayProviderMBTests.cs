using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CameraRayProviderMBTests : TestCollection
{
  [Test]
	public void RequiresCamera()
	{
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<CameraRayProviderMB>();
		Assert.True(camRayProviderMB.TryGetComponent(out Camera _));
	}

  [Test]
	public void ExposesRequiredCamera()
	{
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<CameraRayProviderMB>();
		Assert.AreSame(
			camRayProviderMB.GetComponent<Camera>(),
			camRayProviderMB.Camera
		);
	}
}
