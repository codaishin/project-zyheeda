// using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class BaseInputListenerMB<TInputAction> : MonoBehaviour
	where TInputAction : IInputAction
{
	public BaseInputActionSO<TInputAction>? inputActionSO;

	public UnityEvent<InputAction.CallbackContext> OnInput = new();

	private void Start() {
		this.inputActionSO!.InputAction.AddOnPerformed(this.InvokeOnInput);
	}

	private void InvokeOnInput(InputAction.CallbackContext context) {
		this.OnInput.Invoke(context);
	}
}

public class InputListenerMB : BaseInputListenerMB<InputActionsWrapper> { }
