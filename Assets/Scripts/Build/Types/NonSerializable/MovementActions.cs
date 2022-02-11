public class MovementActions : IMovementActions
{
	public IInputAction Walk =>
		new InputActionsWrapper(this.movement.Walk);
	public IInputAction Run =>
		new InputActionsWrapper(this.movement.Run);

	private PlayerInputConfig.MovementActions movement;

	public MovementActions(PlayerInputConfig.MovementActions movement) {
		this.movement = movement;
	}
}
