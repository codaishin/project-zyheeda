using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class HitMousePositionSOTests : TestCollection
{
	class MockInputConfigSO : BaseInputConfigSO
	{
		private InputAction action = new InputAction(
			binding: "<Mouse>/position",
			type: InputActionType.Value
		);

		public override InputAction this[InputEnum.Action action] => action switch {
			InputEnum.Action.MousePosition => this.action,
			_ => throw new ArgumentException(),
		};

		public override InputActionMap this[InputEnum.Map map] =>
			throw new NotImplementedException();

		private void OnEnable() {
			this.action.Enable();
		}
	}

	private class MockMB : MonoBehaviour { }

	private Mouse? mouse;
	private MockInputConfigSO? inputConfSO;
	private ReferenceSO? cameraRef;
	private Camera? camera;
	private MockMB DefaultMockMB => new GameObject().AddComponent<MockMB>();

	[SetUp]
	public void SetUp() {
		this.mouse = InputSystem.AddDevice<Mouse>();
		this.inputConfSO = ScriptableObject.CreateInstance<MockInputConfigSO>();
		this.cameraRef = ScriptableObject.CreateInstance<ReferenceSO>();
		this.camera = new GameObject().AddComponent<Camera>();
		this.cameraRef.GameObject = this.camera.gameObject;
		this.MouseToScreenMiddle();
	}

	[TearDown]
	public void TearDown() {
		InputSystem.RemoveDevice(this.mouse!);
	}

	private void MouseToScreenMiddle() {
		InputState.Change(
			this.mouse!.position,
			new Vector2(Screen.width / 2, Screen.height / 2)
		);
	}

	[UnityTest]
	public IEnumerator HitNothing() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		hitMouseSO.Try(this.DefaultMockMB).Match(
			some: _ => Assert.Fail("hit something"),
			none: () => Assert.Pass()
		);
	}

	[UnityTest]
	public IEnumerator HitNothingPoint() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		hitMouseSO.TryPoint(this.DefaultMockMB.transform).Match(
			some: _ => Assert.Fail("hit something"),
			none: () => Assert.Pass()
		);
	}

	[UnityTest]
	public IEnumerator HitTarget() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;
		this.camera!.transform.position = Vector3.up;
		this.camera!.transform.LookAt(new Vector3(1, 0, 1));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		hitMouseSO.Try(this.DefaultMockMB.transform).Match(
			some: hit => Assert.AreSame(plane.transform, hit),
			none: () => Assert.Fail("hit nothing")
		);
	}

	[UnityTest]
	public IEnumerator HitTargetPoint() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;
		this.camera!.transform.position = new Vector3(1, 1, 1);
		this.camera!.transform.LookAt(new Vector3(1, 0, 1));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		var expected = new Vector3(1, 0, 1);
		var actual = Vector3.zero;

		hitMouseSO.TryPoint(this.DefaultMockMB.transform).Match(
			some: hit => actual = hit,
			none: () => Assert.Fail("hit nothing")
		);

		Assert.AreEqual(expected.x, actual.x, 0.01f);
		Assert.AreEqual(expected.y, actual.y, 0.01f);
		Assert.AreEqual(expected.z, actual.z, 0.01f);
	}

	[UnityTest]
	public IEnumerator MissTargetLayerMissmatch() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;
		this.camera!.transform.position = Vector3.up;
		this.camera!.transform.LookAt(new Vector3(1, 0, 1));

		plane.gameObject.layer = 30;
		hitMouseSO.constraint = 1 << 15;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		hitMouseSO.Try(this.DefaultMockMB.transform).Match(
			some: hit => Assert.Fail($"hit something {hit} - {hit.gameObject.layer}"),
			none: () => Assert.Pass()
		);
	}

	[UnityTest]
	public IEnumerator MissTargetPointLayerMissmatch() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;
		this.camera!.transform.position = Vector3.up;
		this.camera!.transform.LookAt(new Vector3(1, 0, 1));

		plane.gameObject.layer = 30;
		hitMouseSO.constraint += 1 << 15;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		hitMouseSO.TryPoint(this.DefaultMockMB.transform).Match(
			some: _ => Assert.Fail("hit something"),
			none: () => Assert.Pass()
		);
	}

	[UnityTest]
	public IEnumerator HitTargetLayer() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;
		this.camera!.transform.position = Vector3.up;
		this.camera!.transform.LookAt(new Vector3(1, 0, 1));

		plane.gameObject.layer = 30;
		hitMouseSO.constraint += 1 << 30;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		hitMouseSO.Try(this.DefaultMockMB.transform).Match(
			some: hit => Assert.AreSame(plane.transform, hit),
			none: () => Assert.Fail("hit nothing")
		);
	}

	[UnityTest]
	public IEnumerator HitTargetPointLayer() {
		var hitMouseSO = ScriptableObject.CreateInstance<HitMousePositionSO>();
		var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
		hitMouseSO.cameraSO = this.cameraRef;
		hitMouseSO.inputConfigSO = this.inputConfSO;
		this.camera!.transform.position = new Vector3(1, 1, 1);
		this.camera!.transform.LookAt(new Vector3(1, 0, 1));

		plane.gameObject.layer = 30;
		hitMouseSO.constraint += 1 << 30;

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		var expected = new Vector3(1, 0, 1);
		var actual = Vector3.zero;

		hitMouseSO.TryPoint(this.DefaultMockMB.transform).Match(
			some: hit => actual = hit,
			none: () => Assert.Fail("hit nothing")
		);

		Assert.AreEqual(expected.x, actual.x, 0.01f);
		Assert.AreEqual(expected.y, actual.y, 0.01f);
		Assert.AreEqual(expected.z, actual.z, 0.01f);
	}
}
