using UnityEngine;

public class RayCastHitMB : MonoBehaviour, IRayCastHit
{
	private BaseRayProviderMB? rayProvider;

	public Reference raySource;
	public LayerMask layerConstraints;

	private void Start() {
		this.rayProvider = this
			.raySource
			.GameObject
			.GetComponent<BaseRayProviderMB>();
	}

	public Maybe<RaycastHit> TryHit() =>
		this.Hit(out RaycastHit hit)
			? Maybe.Some(hit)
			: Maybe.None<RaycastHit>();

	private bool Hit(out RaycastHit hit) => this.layerConstraints == default
		? Physics.Raycast(this.rayProvider!.Ray, out hit, float.PositiveInfinity)
		: Physics.Raycast(
			this.rayProvider!.Ray,
			out hit,
			float.PositiveInfinity,
			this.layerConstraints
		);
}
