using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class CameraRayProviderMBTests : TestCollection
{
	class MockMousePositionSO : BaseInputActionSO
	{
		public InputAction? action;
		public override InputAction Action => this.action!;
	}

	private Mouse? mouse;

	[SetUp]
	public void SetUp() {
		this.mouse = InputSystem.AddDevice<Mouse>();
	}

	[TearDown]
	public void TearDown() {
		InputSystem.RemoveDevice(this.mouse!);
	}

	[Test]
	public void BaseRequiresCamera() {
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<CameraRayProviderMB>();
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
			.AddComponent<CameraRayProviderMB>();
		Assert.AreSame(
			camRayProviderMB.GetComponent<Camera>(),
			camRayProviderMB.Camera
		);
	}

	[UnityTest]
	public IEnumerator RayForward() {
		var mockMousePositionSO = ScriptableObject
			.CreateInstance<MockMousePositionSO>();
		var camRayProviderMB = new GameObject("cam")
			.AddComponent<CameraRayProviderMB>();
		var camera = camRayProviderMB.Camera!;

		var action = new InputAction(
			binding: "<Mouse>/position",
			type: InputActionType.Value,
			expectedControlType: "Vector2(normalize=false)"
		);
		mockMousePositionSO.action = action;
		camRayProviderMB.mousePosition = mockMousePositionSO;
		camera.nearClipPlane = 0.01f;

		action.Enable();

		InputState.Change(
			this.mouse!.position,
			new Vector2(Screen.width / 2, Screen.height / 2)
		);

		yield return new WaitForEndOfFrame();

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
