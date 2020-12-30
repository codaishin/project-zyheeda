using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRayProviderMB : BaseRayProviderMB
{
	public BaseMouseSO mouseSO;

	public Camera Camera { get; private set; }
	public override Ray Ray => this.Camera.ScreenPointToRay(this.mouseSO.Position);

	private void Awake() => this.Camera = this.GetComponent<Camera>();
}
