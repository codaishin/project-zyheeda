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
		if (this.Hit(out T target)) {
			this.onHit.Invoke(target);
		}
	}

	private bool Hit(out T target)
	{
		target = default;
		return  this.Hit(out RaycastHit hit) && this.Get(hit, out target);
	}

	private bool Hit(out RaycastHit hit) => this.layerConstraints == default
		? Physics.Raycast(this.rayProvider.Ray, out hit, float.PositiveInfinity)
		: Physics.Raycast(this.rayProvider.Ray, out hit, float.PositiveInfinity, this.layerConstraints);

	public abstract bool Get(RaycastHit hit, out T got);
}
