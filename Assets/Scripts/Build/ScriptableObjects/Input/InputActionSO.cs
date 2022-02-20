using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseInputActionSO : ScriptableObject
{
	public abstract InputAction Action { get; }
}

public class InputActionSO : BaseInputActionSO
{
	public PlayerInputConfigSO? playerConfigSO;
	public InputOption inputOption;

	public override InputAction Action => this.inputOption switch {
		InputOption.Walk => this.playerConfigSO!.Config.Movement.Walk,
		InputOption.Run => this.playerConfigSO!.Config.Movement.Run,
		InputOption.MousePosition => this.playerConfigSO!.Config.Mouse.Position,
		_ => throw new ArgumentException(),
	};
}
