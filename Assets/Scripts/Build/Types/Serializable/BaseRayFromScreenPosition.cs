using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public abstract class BaseRayFromScreenPosition : IRay
{
	private InputAction? mousePosition;
	private Camera? cam;

	public ReferenceSO? camera;
	public BaseInputConfigSO? inputConfigSO;

	private Camera Cam =>
		this.cam ?? this.GetCam();
	private InputAction MousePostion =>
		this.mousePosition ?? this.GetMousePosition();

	public Ray Ray =>
		this
			.Cam
			.ScreenPointToRay(this.MousePostion.ReadValue<Vector2>());

	private Camera GetCam() {
		this.cam = this.camera!.GameObject!.RequireComponent<Camera>();
		return this.cam;
	}

	private InputAction GetMousePosition() {
		this.mousePosition = this.inputConfigSO![InputEnum.Action.MousePosition];
		return this.mousePosition;
	}
}
