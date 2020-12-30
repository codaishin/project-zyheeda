using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CameraRayProviderMBTests : TestCollection
{
	private class MockMouseSO : BaseMouseSO
	{
		public Vector3 position;

		public override Vector3 Position => this.position;
	}

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

	[Test]
	public void RayForward()
	{
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<CameraRayProviderMB>();
		var camera = camRayProviderMB.Camera;
		var mockMouseSO = ScriptableObject.CreateInstance<MockMouseSO>();

		mockMouseSO.position = new Vector3(Screen.width / 2, Screen.height / 2);
		camRayProviderMB.mouseSO = mockMouseSO;
		camera.nearClipPlane = 0.01f;

		Tools.AssertEqual(
			Vector3.forward,
			camRayProviderMB.Ray.direction,
			delta: new Vector3(0.005f, 0.005f, 0.005f)
		);
		Tools.AssertEqual(
			new Vector3(0, 0, 0.01f),
			camRayProviderMB.Ray.origin,
			delta: new Vector3(0.005f, 0.005f, 0)
		);
	}
}
