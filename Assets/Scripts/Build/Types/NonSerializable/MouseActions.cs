public class MouseActions : IMouseActions
{
	public IInputAction Position => new InputActionsWrapper(this.mouse.Position);

	private PlayerInputConfig.MouseActions mouse;

	public MouseActions(PlayerInputConfig.MouseActions mouse) {
		this.mouse = mouse;
	}
}
