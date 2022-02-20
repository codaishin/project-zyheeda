// using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputListenerMB : MonoBehaviour
{
	public BaseInputActionSO? inputActionSO;
	public UnityEvent<InputAction.CallbackContext>? OnInput;

	private bool triggeredThisFrame = false;

	private void Start() {
		this.inputActionSO!.Action.performed += this.InvokeOnInput;
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
