using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class InputListenerMBTests : TestCollection
{
	class MockInputActionSO : BaseInputActionSO
	{
		public InputAction? action;
		public override InputAction Action => this.action!;
	}

	private Gamepad? pad;

	[SetUp]
	public void SetUp() {
		this.pad = InputSystem.AddDevice<Gamepad>();
	}

	[TearDown]
	public void TearDown() {
		InputSystem.RemoveDevice(this.pad!);
	}

	[UnityTest]
	public IEnumerator CallOnInput() {
		var called = 0;
		var so = ScriptableObject.CreateInstance<MockInputActionSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		mb.inputActionSO = so;
		so.action = new InputAction(
			binding: "<Gamepad>/leftTrigger",
			type: InputActionType.Button
		);
		so.action.Enable();

		yield return new WaitForEndOfFrame();

		mb.OnInput!.AddListener(_ => ++called);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator CallOnInputOnlyOncePerFrame() {
		var called = 0;
		var so = ScriptableObject.CreateInstance<MockInputActionSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		mb.inputActionSO = so;
		so.action = new InputAction(
			binding: "<Gamepad>/leftTrigger",
			type: InputActionType.Button
		);
		so.action.Enable();

		yield return new WaitForEndOfFrame();

		mb.OnInput!.AddListener(_ => ++called);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(1, called);
	}

	[UnityTest]
	public IEnumerator CallOnInputNextFrame() {
		var called = 0;
		var so = ScriptableObject.CreateInstance<MockInputActionSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		mb.inputActionSO = so;
		so.action = new InputAction(
			binding: "<Gamepad>/leftTrigger",
			type: InputActionType.Button
		);
		so.action.Enable();

		yield return new WaitForEndOfFrame();

		mb.OnInput!.AddListener(_ => ++called);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0f }
		);

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0f }
		);

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(2, called);
	}
}
