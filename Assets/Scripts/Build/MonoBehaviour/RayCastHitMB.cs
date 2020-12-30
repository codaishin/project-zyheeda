using UnityEngine;

public class RayCastHitMB : MonoBehaviour
{
	public BaseRayProviderMB rayProviderMB;
	public LayerMask layerConstraints;

	[Header("On Hit Callbacks")]
	public GameObjectEvent onHitObject;
	public Vector3Event onHitPoint;

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
		? Physics.Raycast(this.rayProviderMB.Ray, out hit, float.MaxValue)
		: Physics.Raycast(this.rayProviderMB.Ray, out hit, float.MaxValue, this.layerConstraints);
}
