using System;
using UnityEngine;

[Serializable]
public abstract class BaseRayFromScreenPosition<TScreenPosition> :
	IRay
	where TScreenPosition :
		IPosition
{
	public ReferenceSO? camera;
	public TScreenPosition? screenPosition;

	public Ray Ray {
		get {
			if (this.camera == null) throw this.NullError();
			if (this.screenPosition == null) throw this.NullError();
			return this.camera.GameObject!
				.GetComponent<Camera>()
				.ScreenPointToRay(this.screenPosition.Position);
		}
	}
}
