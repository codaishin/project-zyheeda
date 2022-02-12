using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRayProviderMBTests : TestCollection
{
	class MockInputAction : IInputAction
	{
		public Vector2 position;

		public void AddOnPerformed(Action<InputAction.CallbackContext> listener) =>
			throw new NotImplementedException();
		public TValue ReadValue<TValue>() where TValue : struct =>
			this.position is TValue value
				? value
				: new TValue();
	}

	class MockMousePositionCenterSO : BaseInputActionSO<MockInputAction>
	{
		public MockInputAction inputAction = new MockInputAction {
			position = new Vector2(Screen.width / 2, Screen.height / 2)
		};

		protected override MockInputAction Wrap(InputAction _) =>
			this.inputAction;

		protected override void OnEnable() {
			this.config = ScriptableObject.CreateInstance<PlayerInputConfigSO>();
			base.OnEnable();
		}
	}

	class MockCameraRayProviderMB : BaseCameraRayProviderMB<MockInputAction> { }

	[Test]
	public void BaseRequiresCamera() {
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<MockCameraRayProviderMB>();
		Assert.True(camRayProviderMB.TryGetComponent(out Camera _));
	}

	[Test]
	public void RequiresCamera() {
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<CameraRayProviderMB>();
		Assert.True(camRayProviderMB.TryGetComponent(out Camera _));
	}

	[Test]
	public void ExposesRequiredCamera() {
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<MockCameraRayProviderMB>();
		Assert.AreSame(
			camRayProviderMB.GetComponent<Camera>(),
			camRayProviderMB.Camera
		);
	}

	[Test]
	public void RayForward() {
		var mockMousePositionSO = ScriptableObject
			.CreateInstance<MockMousePositionCenterSO>();
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<MockCameraRayProviderMB>();
		var camera = camRayProviderMB.Camera!;

		camRayProviderMB.mousePosition = mockMousePositionSO;
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
