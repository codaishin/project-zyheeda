using UnityEngine;
using UnityEngine.Events;

public abstract class BaseRayCastHitMB<T> : MonoBehaviour
{
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
		if (this.Hit(out RaycastHit hit)) {
			this.onHit.Invoke(this.Get(hit));
		}
	}

	private bool Hit(out RaycastHit hit) => this.layerConstraints == default
		? Physics.Raycast(this.rayProvider.Ray, out hit, float.MaxValue)
		: Physics.Raycast(this.rayProvider.Ray, out hit, float.MaxValue, this.layerConstraints);

	public abstract T Get(RaycastHit hit);
}
