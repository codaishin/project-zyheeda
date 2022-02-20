using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class HoldRepeatInteractionTests : TestCollection
{
	private Gamepad? pad;

	[SetUp]
	public void SetUp() {
		this.pad = InputSystem.AddDevice<Gamepad>("Mock Pad");
	}

	[TearDown]
	public void TearDown() {
		InputSystem.RemoveDevice(this.pad!);
	}

	[UnityTest]
	public IEnumerator Start() {
		var started = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4)"
		);
		action.started += _ => started = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForEndOfFrame();

		Assert.True(started);
	}

	[UnityTest]
	public IEnumerator StartPressPoint() {
		var started = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4)"
		);
		action.started += _ => started = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0.3f }
		);

		yield return new WaitForEndOfFrame();

		Assert.False(started);
	}

	[UnityTest]
	public IEnumerator StartPressPointValue() {
		var started = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.5)"
		);
		action.started += _ => started = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0.4f }
		);

		yield return new WaitForEndOfFrame();

		Assert.False(started);
	}

	[UnityTest]
	public IEnumerator Performed() {
		var performed = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4)"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.6f);

		Assert.True(performed);
	}

	[UnityTest]
	public IEnumerator PerformedHoldTime() {
		var performed = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.2, pressPoint=0.4)"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.3f);

		Assert.True(performed);
	}

	[UnityTest]
	public IEnumerator RepeatedlyPerformed() {
		var canceled = false;
		var performedCount = 0;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4)"
		);
		action.performed += _ => ++performedCount;
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.6f);

		performedCount = 0;

		yield return new WaitForSeconds(0.02f);
		yield return new WaitForSeconds(0.02f);
		yield return new WaitForSeconds(0.02f);

		Assert.AreEqual(3, performedCount);
		Assert.False(canceled);
	}

	[UnityTest]
	public IEnumerator RepeatedlyPerformedFrequency() {
		var canceled = false;
		var performedCount = 0;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4, frequency=3)"
		);
		action.performed += _ => ++performedCount;
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.6f);

		performedCount = 0;

		yield return new WaitForSeconds(0.4f);

		Assert.AreEqual(1, performedCount);
		Assert.False(canceled);
	}

	[UnityTest]
	public IEnumerator Cancel() {
		var canceled = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4)"
		);
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.6f);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0f }
		);

		yield return new WaitForSeconds(0.2f);

		Assert.True(canceled);
	}

	[UnityTest]
	public IEnumerator CancelPressPoint() {
		var canceled = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4)"
		);
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.6f);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0.3f }
		);

		yield return new WaitForSeconds(0.2f);

		Assert.True(canceled);
	}

	[UnityTest]
	public IEnumerator CancelPressPointValue() {
		var canceled = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.7)"
		);
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.6f);

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 0.6f }
		);

		yield return new WaitForSeconds(0.2f);

		Assert.True(canceled);
	}

	[UnityTest]
	public IEnumerator NotPerformed() {
		var performed = false;
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "repeatHold(holdTime=0.5, pressPoint=0.4)"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(
			this.pad!,
			new GamepadState { leftTrigger = 1f }
		);

		yield return new WaitForSeconds(0.4f);

		Assert.False(performed);
	}
}
