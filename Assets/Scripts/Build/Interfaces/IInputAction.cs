using System;
using UnityEngine.InputSystem;

public interface IInputAction
{
	public void AddOnPerformed(Action<InputAction.CallbackContext> listener);
	public void AddOnCanceled(Action<InputAction.CallbackContext> listener);
	public void AddOnStarted(Action<InputAction.CallbackContext> listener);
	public TValue ReadValue<TValue>() where TValue : struct;
}
