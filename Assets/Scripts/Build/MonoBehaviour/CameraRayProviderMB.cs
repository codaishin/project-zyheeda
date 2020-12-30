using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraRayProviderMB : BaseRayProviderMB
{
	public Camera Camera { get; private set; }
	public BaseMouseSO mouseSO;
	public override Ray Ray => this.Camera.ScreenPointToRay(this.mouseSO.Position);

	private void Awake() => this.Camera = this.GetComponent<Camera>();
}
