using System;
using UnityEngine.InputSystem;

public class InputActionsWrapper : IInputAction
{
	private InputAction action;

	public InputActionsWrapper(InputAction action) => this.action = action;

	public void AddOnCanceled(Action<InputAction.CallbackContext> listener) =>
		this.action.canceled += listener;
	public void AddOnPerformed(Action<InputAction.CallbackContext> listener) =>
		this.action.performed += listener;
	public void AddOnStarted(Action<InputAction.CallbackContext> listener) =>
		this.action.started += listener;
	public TValue ReadValue<TValue>() where TValue : struct =>
		this.action.ReadValue<TValue>();
}
