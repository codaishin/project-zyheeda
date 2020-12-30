using UnityEngine;

public class RayCastHitMB : MonoBehaviour
{
	private BaseRayProviderMB buffer;

	public Reference rayProviderReference;
	public LayerMask layerConstraints;

	[Header("On Hit Callbacks")]
	public GameObjectEvent onHitObject;
	public Vector3Event onHitPoint;

	private Ray Ray => this.rayProviderReference.Get(ref this.buffer).Ray;

	private void Start()
	{
		if (this.onHitObject == null) {
			this.onHitObject = new GameObjectEvent();
		}
		if (this.onHitPoint == null) {
			this.onHitPoint = new Vector3Event();
		}
	}

	public void TryHit()
	{
		if (this.Hit(out RaycastHit hit)) {
			this.onHitObject.Invoke(hit.transform.gameObject);
			this.onHitPoint.Invoke(hit.point);
		}
	}

	private bool Hit(out RaycastHit hit) => this.layerConstraints == default
		? Physics.Raycast(this.Ray, out hit, float.MaxValue)
		: Physics.Raycast(this.Ray, out hit, float.MaxValue, this.layerConstraints);
}
