using System;
using UnityEngine;

[Serializable]
public abstract class BaseRayFromScreenPosition<TInputAction> :
	IRay
	where TInputAction :
		IInputAction
{
	public ReferenceSO? camera;
	public BaseInputActionSO<TInputAction>? screenPosition;

	public Ray Ray =>
		this
			.camera!
			.GameObject!
			.GetComponent<Camera>()
			.ScreenPointToRay(this.screenPosition!.InputAction.ReadValue<Vector2>());
}
