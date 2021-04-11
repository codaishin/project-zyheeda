using System;
using UnityEngine;

[Serializable]
public abstract class BaseRayFromScreenPosition<TScreenPosition> : IRay
	where TScreenPosition : IPosition
{
	public ReferenceSO camera;
	public TScreenPosition screenPosition;

	public Ray Ray => this.camera.GameObject
		.GetComponent<Camera>()
		.ScreenPointToRay(this.screenPosition.Position);
}
