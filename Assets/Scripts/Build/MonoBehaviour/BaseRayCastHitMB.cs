using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseRayCastHitMB<T> : MonoBehaviour
{
	[Serializable]
	public class OnHitEvent : UnityEvent<T> {}

	private BaseRayProviderMB rayProvider;

	public Reference raySource;
	public LayerMask layerConstraints;
	public OnHitEvent onHit;

	private void Start()
	{
		if (this.onHit == null) {
			this.onHit = new OnHitEvent();
		}
		this.rayProvider = this.raySource
			.GameObject
			.GetComponent<BaseRayProviderMB>();
	}

	public void TryHit()
	{
		if (this.Hit(out RaycastHit hit) && this.Get(hit, out T got)) {
			this.onHit.Invoke(got);
		}
	}

	private bool Hit(out RaycastHit hit) => this.layerConstraints == default
		? Physics.Raycast(this.rayProvider.Ray, out hit, float.MaxValue)
		: Physics.Raycast(this.rayProvider.Ray, out hit, float.MaxValue, this.layerConstraints);

	public abstract bool Get(RaycastHit hit, out T got);
}
