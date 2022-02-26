using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class MultiTapHoldRepeatInteractionTests : TestCollection
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
		var press = new GamepadState { leftTrigger = 1f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=2," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.5," +
					"pressPoint=0.4" +
				")"
		);
		action.started += _ => started = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForEndOfFrame();

		Assert.True(started);
	}

	[UnityTest]
	public IEnumerator NoStartWithTooLowPress() {
		var started = false;
		var press = new GamepadState { leftTrigger = 0.3f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=2," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.5," +
					"pressPoint=0.4" +
				")"
		);
		action.started += _ => started = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForEndOfFrame();

		Assert.False(started);
	}

	[UnityTest]
	public IEnumerator PerformedWithTapCountZero() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=0," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.4" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.3f);

		Assert.True(performed);
	}

	[UnityTest]
	public IEnumerator PerformedWithTapCount1() {
		var performed = false;
		var started = 0;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=1," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.4" +
				")"
		);
		action.started += _ => ++started;
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual((1, false), (started, performed));

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual((1, false), (started, performed));

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.21f);

		Assert.AreEqual((1, true), (started, performed));
	}

	[UnityTest]
	public IEnumerator PerformedWithTapCount2() {
		var performed = false;
		var started = 0;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=2," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.4" +
				")"
		);
		action.started += _ => ++started;
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual((1, false), (started, performed));

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual((1, false), (started, performed));

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		Assert.AreEqual((1, false), (started, performed));

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.21f);

		Assert.AreEqual((1, true), (started, performed));
	}

	[UnityTest]
	public IEnumerator NoPerformedWithTapCount2WithPartialTapPress() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var press50 = new GamepadState { leftTrigger = 0.5f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=2," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.8" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, press50);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.21f);

		Assert.False(performed);
	}

	[UnityTest]
	public IEnumerator NoPerformedWithTooLongTap() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=1," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.8" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.3f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.21f);

		Assert.False(performed);
	}


	[UnityTest]
	public IEnumerator CanceledWithTooLongTap() {
		var canceled = false;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=1," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.8" +
				")"
		);
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.3f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		Assert.True(canceled);
	}

	[UnityTest]
	public IEnumerator NoPerformedWithTooLongTapSpacingBetweenTaps() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=2," +
					"maxTapSpacing=0.1," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.8" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.3f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.21f);

		Assert.False(performed);
	}

	[UnityTest]
	public IEnumerator CanceledWithTooLongTapSpacingBetweenTaps() {
		var canceled = false;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=2," +
					"maxTapSpacing=0.1," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.8" +
				")"
		);
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.3f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		Assert.True(canceled);
	}

	[UnityTest]
	public IEnumerator NoPerformedWithTooLongTapSpacingBetweenTapAndHold() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=1," +
					"maxTapSpacing=0.2," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.8" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.3f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.21f);

		Assert.False(performed);
	}

	[UnityTest]
	public IEnumerator CanceledWithTooLongTapSpacingBetweenTapAndHold() {
		var canceled = false;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=1," +
					"maxTapSpacing=0.2," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.8" +
				")"
		);
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.3f);

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.21f);

		Assert.True(canceled);
	}


	[UnityTest]
	public IEnumerator PerformedRepeatWithTapCountZero() {
		var performed = 0;
		var press = new GamepadState { leftTrigger = 1f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=0," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.1," +
					"pressPoint=0.4," +
					"frequency=5" +
				")"
		);
		action.performed += _ => ++performed;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.51f);

		Assert.AreEqual(3, performed);
	}

	[UnityTest]
	public IEnumerator NoPerformedWithTapCountZeroAndTooLowPress() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var pressTooLow = new GamepadState { leftTrigger = 0.2f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=0," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.4" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.01f);

		InputSystem.QueueStateEvent(this.pad!, pressTooLow);

		yield return new WaitForSeconds(0.2f);

		Assert.False(performed);
	}

	[UnityTest]
	public IEnumerator NoPerformedWithTapCountZeroAndTooShortHold() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=0," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.4" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.11f);

		Assert.False(performed);
	}

	[UnityTest]
	public IEnumerator CancelWithTapCountZeroAndTooShortHold() {
		var canceled = false;
		var press = new GamepadState { leftTrigger = 1f };
		var release = new GamepadState { leftTrigger = 0f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=0," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.4" +
				")"
		);
		action.canceled += _ => canceled = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.11f);

		InputSystem.QueueStateEvent(this.pad!, release);

		yield return new WaitForSeconds(0.1f);

		Assert.True(canceled);
	}

	[UnityTest]
	public IEnumerator
	NoPerformedWithTapCountZeroAndPartialReleaseWithinHoldTime() {
		var performed = false;
		var press = new GamepadState { leftTrigger = 1f };
		var press50 = new GamepadState { leftTrigger = 0.5f };
		var action = new InputAction(
			type: InputActionType.Button,
			binding: "<Gamepad>/leftTrigger",
			interactions: "multiTapHoldRepeat(" +
					"tapCount=0," +
					"maxTapSpacing=0.75," +
					"maxTapDuration=0.2," +
					"holdTime=0.2," +
					"pressPoint=0.4" +
				")"
		);
		action.performed += _ => performed = true;
		action.Enable();

		yield return new WaitForEndOfFrame();

		InputSystem.QueueStateEvent(this.pad!, press);

		yield return new WaitForSeconds(0.1f);

		InputSystem.QueueStateEvent(this.pad!, press50);

		yield return new WaitForSeconds(0.01f);

		Assert.False(performed);
	}
}
