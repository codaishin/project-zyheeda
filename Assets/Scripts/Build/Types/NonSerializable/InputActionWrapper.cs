using System;
using UnityEngine.InputSystem;

public class InputActionsWrapper : IInputAction
{
	private InputAction action;

	public InputActionsWrapper(InputAction action) => this.action = action;

	public void AddOnPerformed(Action<InputAction.CallbackContext> listener) =>
		this.action.performed += listener;
	public TValue ReadValue<TValue>() where TValue : struct =>
		this.action.ReadValue<TValue>();
}
