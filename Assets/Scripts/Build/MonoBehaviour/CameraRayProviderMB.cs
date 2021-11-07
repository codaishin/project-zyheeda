using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRayProviderMB : BaseRayProviderMB
{
	public BaseMouseSO? mouseSO;

	public Camera? Camera { get; private set; }

	public override Ray Ray {
		get {
			if (this.mouseSO == null) throw this.NullError();
			return this.Camera!.ScreenPointToRay(this.mouseSO.Position);
		}
	}

	private void Awake() => this.Camera = this.GetComponent<Camera>();
}
