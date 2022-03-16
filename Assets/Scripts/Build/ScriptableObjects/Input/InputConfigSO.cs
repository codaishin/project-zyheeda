using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseInputConfigSO : ScriptableObject
{
	public abstract InputAction this[InputEnum.Action action] { get; }
	public abstract InputActionMap this[InputEnum.Map map] { get; }
}

[CreateAssetMenu(menuName = "ScriptableObjects/InputConfig")]
public class InputConfigSO : BaseInputConfigSO
{
	private PlayerInputConfig? config;

	public PlayerInputConfig Config => this.config ?? this.NewConfig();

	public override InputActionMap this[InputEnum.Map map] => map switch {
		InputEnum.Map.Movement => this.Config.Movement.Get(),
		InputEnum.Map.Mouse => this.Config.Mouse.Get(),
		_ => throw new ArgumentException(),
	};

	public override InputAction this[InputEnum.Action action] => action switch {
		InputEnum.Action.Walk => this.Config.Movement.Walk,
		InputEnum.Action.Run => this.Config.Movement.Run,
		InputEnum.Action.WalkOrRun => this.Config.Movement.WalkOrRun,
		InputEnum.Action.MousePosition => this.Config.Mouse.Position,
		_ => throw new ArgumentException(),
	};

	private PlayerInputConfig NewConfig() {
		this.config = new PlayerInputConfig();
		return this.config;
	}
}
