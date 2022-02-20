using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRayProviderMB : BaseRayProviderMB
{
	public BaseInputActionSO? mousePosition;

	public Camera? Camera { get; private set; }

	public override Ray Ray =>
		this
			.Camera!
			.ScreenPointToRay(this.mousePosition!.Action.ReadValue<Vector2>());

	private void Awake() => this.Camera = this.GetComponent<Camera>();
}
