using System;
using UnityEngine;

[Serializable]
public abstract class BaseRayFromScreenPosition : IRay
{
	public ReferenceSO? camera;
	public BaseInputActionSO? screenPosition;

	public Ray Ray =>
		this
			.camera!
			.GameObject!
			.GetComponent<Camera>()
			.ScreenPointToRay(this.screenPosition!.Action.ReadValue<Vector2>());
}
