using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class BaseRayFromScreenPositionTests : TestCollection
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

	private class MockRayCamToPos : BaseRayFromScreenPosition<MockInputAction> { }

	[UnityTest]
	public IEnumerator GetRayDirection() {
		var so = ScriptableObject.CreateInstance<MockMousePositionCenterSO>();
		var ray = new MockRayCamToPos {
			camera = ScriptableObject.CreateInstance<ReferenceSO>(),
			screenPosition = so,
		};
		var cam = new GameObject("cam").AddComponent<Camera>();
		ray.camera.GameObject = cam.gameObject;

		cam.transform.position = Vector3.up;
		cam.nearClipPlane = 0.01f;

		yield return new WaitForEndOfFrame();

		Tools.AssertEqual(
			Vector3.forward,
			ray.Ray.direction,
			delta: new Vector3(0.005f, 0.005f, 0.005f)
		);
	}

	[UnityTest]
	public IEnumerator GetRayOrigin() {
		var so = ScriptableObject.CreateInstance<MockMousePositionCenterSO>();
		var ray = new MockRayCamToPos {
			camera = ScriptableObject.CreateInstance<ReferenceSO>(),
			screenPosition = so,
		};
		var cam = new GameObject("cam").AddComponent<Camera>();
		ray.camera.GameObject = cam.gameObject;

		so.inputAction = new MockInputAction {
			position = new Vector2(Screen.width / 2, Screen.height / 2)
		};
		cam.transform.position = Vector3.up;
		cam.nearClipPlane = 0.01f;

		yield return new WaitForEndOfFrame();

		Tools.AssertEqual(
			Vector3.up,
			ray.Ray.origin,
			delta: new Vector3(0.005f, 0.005f, 0.01f)
		);
	}
}
