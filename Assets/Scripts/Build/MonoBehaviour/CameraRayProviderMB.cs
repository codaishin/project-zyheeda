using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class BaseCameraRayProviderMB<TInputAction> : BaseRayProviderMB
	where TInputAction : IInputAction
{
	public BaseInputActionSO<TInputAction>? mousePosition;

	public Camera? Camera { get; private set; }

	public override Ray Ray =>
		this
			.Camera!
			.ScreenPointToRay(this.mousePosition!.InputAction.ReadValue<Vector2>());

	private void Awake() => this.Camera = this.GetComponent<Camera>();
}

[RequireComponent(typeof(Camera))]
public class CameraRayProviderMB :
	BaseCameraRayProviderMB<InputActionsWrapper>
{ }
