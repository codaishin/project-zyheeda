using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseInputActionSO<TInputAction> : ScriptableObject
{
	private TInputAction? inputAction;

	public InputOption inputOption;
	public TInputAction InputAction => this.inputAction!;
	public PlayerInputConfigSO? config;

	protected abstract TInputAction Wrap(InputAction action);

	private InputAction GetPlayerAction(PlayerInputConfig playerInput) {
		return this.inputOption switch {
			InputOption.Walk => playerInput.Movement.Walk,
			InputOption.Run => playerInput.Movement.Run,
			InputOption.MousePosition => playerInput.Mouse.Position,
			_ => throw new ArgumentException(
				$"No action matching for {this.inputOption}"
			),
		};
	}

	protected virtual void OnEnable() {
		InputAction playerAction = this.GetPlayerAction(this.config!.Config);
		this.inputAction = this.Wrap(playerAction);
	}
}

[CreateAssetMenu(menuName = "ScriptableObjects/InputAction")]
public class InputActionSO : BaseInputActionSO<InputActionsWrapper>
{
	protected override InputActionsWrapper Wrap(InputAction action) =>
		new InputActionsWrapper(action);
}
