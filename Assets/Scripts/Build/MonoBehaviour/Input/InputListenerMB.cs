// using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputListenerMB : MonoBehaviour
{
	public BaseInputConfigSO? inputConfigSO;
	public InputEnum.Action action;
	public UnityEvent<InputAction.CallbackContext>? OnInput;

	private bool triggeredThisFrame = false;

	private void Start() {
		InputAction action = this.inputConfigSO![this.action];
		action.performed += this.InvokeOnInput;
		if (this.OnInput == null) {
			this.OnInput = new();
		}
	}

	private void LateUpdate() {
		this.triggeredThisFrame = false;
	}

	private void InvokeOnInput(InputAction.CallbackContext context) {
		if (!this.triggeredThisFrame) {
			this.OnInput!.Invoke(context);
			this.triggeredThisFrame = true;
		}
	}
}
