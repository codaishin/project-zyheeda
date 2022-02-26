using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class InputListenerMBTests : TestCollection
{
	class MockInputConfigSO : BaseInputConfigSO
	{
		public InputAction action = new InputAction(
			binding: "<Gamepad>/leftTrigger",
			type: InputActionType.Button
		);
		public InputEnum.Action onlyAction = InputEnum.Action.Walk;

		public override InputAction this[InputEnum.Action action] => action switch {
			InputEnum.Action when action == this.onlyAction => this.action,
			_ => throw new ArgumentException(),
		};

		public override InputActionMap this[InputEnum.Map map] =>
			throw new NotImplementedException();
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
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		mb.inputConfigSO = so;
		mb.action = InputEnum.Action.Walk;
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
	public IEnumerator CallOnInputRun() {
		var called = 0;
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		mb.inputConfigSO = so;
		mb.action = InputEnum.Action.Run;
		so.onlyAction = InputEnum.Action.Run;
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
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		mb.inputConfigSO = so;
		mb.action = InputEnum.Action.Walk;
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
		var so = ScriptableObject.CreateInstance<MockInputConfigSO>();
		var mb = new GameObject("obj").AddComponent<InputListenerMB>();
		mb.inputConfigSO = so;
		mb.action = InputEnum.Action.Walk;
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
