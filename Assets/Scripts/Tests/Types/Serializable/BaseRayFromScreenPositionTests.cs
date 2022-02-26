using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class BaseRayFromScreenPositionTests : TestCollection
{
	class MockInputConfigSO : BaseInputConfigSO
	{
		public InputAction action = new InputAction(
			binding: "<Mouse>/position",
			type: InputActionType.Value
		);

		public override InputAction this[InputEnum.Action action] => action switch {
			InputEnum.Action when action == InputEnum.Action.MousePosition =>
				this.action,
			_ =>
				throw new ArgumentException(),
		};

		public override InputActionMap this[InputEnum.Map map] =>
			throw new NotImplementedException();
	}

	private class MockRayCamToPos : BaseRayFromScreenPosition { }

	private Mouse? mouse;

	[SetUp]
	public void SetUp() {
		this.mouse = InputSystem.AddDevice<Mouse>();
	}

	[TearDown]
	public void TearDown() {
		InputSystem.RemoveDevice(this.mouse!);
	}

	[UnityTest]
	public IEnumerator GetRayDirection() {
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var ray = new MockRayCamToPos {
			camera = ScriptableObject.CreateInstance<ReferenceSO>(),
			inputConfigSO = so,
		};
		var cam = new GameObject("cam").AddComponent<Camera>();
		ray.camera.GameObject = cam.gameObject;
		cam.transform.position = Vector3.up;
		cam.nearClipPlane = 0.01f;
		so.action.Enable();

		InputState.Change(
			this.mouse!.position,
			new Vector2(Screen.width / 2, Screen.height / 2)
		);

		yield return new WaitForEndOfFrame();

		Tools.AssertEqual(
			Vector3.forward,
			ray.Ray.direction,
			delta: new Vector3(0.005f, 0.005f, 0.005f)
		);
	}

	[UnityTest]
	public IEnumerator GetRayOrigin() {
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var ray = new MockRayCamToPos {
			camera = ScriptableObject.CreateInstance<ReferenceSO>(),
			inputConfigSO = so,
		};
		var cam = new GameObject("cam").AddComponent<Camera>();
		ray.camera.GameObject = cam.gameObject;
		cam.transform.position = Vector3.up;
		cam.nearClipPlane = 0.01f;
		so.action.Enable();

		InputState.Change(
			this.mouse!.position,
			new Vector2(Screen.width / 2, Screen.height / 2)
		);

		yield return new WaitForEndOfFrame();

		Tools.AssertEqual(
			Vector3.up,
			ray.Ray.origin,
			delta: new Vector3(0.005f, 0.005f, 0.01f)
		);
	}
}
