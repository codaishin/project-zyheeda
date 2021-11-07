using UnityEngine;

public class RayCastHitMB : MonoBehaviour
{
	private BaseRayProviderMB? rayProvider;

	public Reference raySource;
	public LayerMask layerConstraints;
	public RayCastEvent? onHit;

	private void Start() {
		if (this.onHit == null) this.onHit = new RayCastEvent();
		this.rayProvider = this.raySource
			.GameObject
			.GetComponent<BaseRayProviderMB>();
	}

	public void TryHit() {
		if (this.Hit(out RaycastHit target)) {
			this.onHit!.Invoke(target);
		}
	}

	private bool Hit(out RaycastHit hit) => this.layerConstraints == default
		? Physics.Raycast(this.rayProvider!.Ray, out hit, float.PositiveInfinity)
		: Physics.Raycast(
			this.rayProvider!.Ray,
			out hit,
			float.PositiveInfinity,
			this.layerConstraints
		);
}
