using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraRayProviderMB : BaseRayProviderMB
{
	private InputAction? mousePosition;
	public BaseInputConfigSO? inputConfigSO;
	public Camera? Camera { get; private set; }

	public override Ray Ray =>
		this
			.Camera!
			.ScreenPointToRay(this.mousePosition!.ReadValue<Vector2>());

	private void Awake() {
		this.Camera = this.GetComponent<Camera>();
	}

	private void Start() {
		this.mousePosition = this.inputConfigSO![InputEnum.Action.MousePosition];
	}
}
