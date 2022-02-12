using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class BaseInputListenerMB<TInputAction> : MonoBehaviour
	where TInputAction : IInputAction
{
	public InputOption input;

	public UnityEvent<InputAction.CallbackContext> OnInput = new();

	private InputAction GetPlayerAction(PlayerInputConfig playerInput) {
		return this.input switch {
			InputOption.Walk => playerInput.Movement.Walk,
			InputOption.Run => playerInput.Movement.Run,
			_ => throw new ArgumentException($"No action amtching for {this.input}"),
		};
	}

	private void Start() {
		PlayerInputConfig playerInput = new PlayerInputConfig();
		InputAction action = this.GetPlayerAction(playerInput);
		TInputAction wrapper = this.GetWrapper(action);
		wrapper.AddOnPerformed(this.InvokeOnInput);
	}

	private void InvokeOnInput(InputAction.CallbackContext context) {
		this.OnInput.Invoke(context);
	}

	protected abstract TInputAction GetWrapper(InputAction action);
}

public class InputListenerMB : BaseInputListenerMB<InputActionsWrapper>
{
	protected override InputActionsWrapper GetWrapper(InputAction action) =>
		new InputActionsWrapper(action);
}
