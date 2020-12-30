using UnityEngine;

public class RayCastHitMB : MonoBehaviour
{
	public BaseRayProviderMB rayProviderMB;
	public LayerMask layers;

	[Header("On Hit Callbacks")]
	public GameObjectEvent onHitGameObject;
	public Vector3Event onHitVector3;

	private void Start()
	{
		if (this.onHitGameObject == null) {
			this.onHitGameObject = new GameObjectEvent();
		}
		if (this.onHitVector3 == null) {
			this.onHitVector3 = new Vector3Event();
		}
	}

	public void TryHit()
	{
		if (this.Hit(out RaycastHit hit)) {
			this.onHitGameObject.Invoke(hit.transform.gameObject);
			this.onHitVector3.Invoke(hit.point);
		}
	}

	private bool Hit(out RaycastHit hit) => this.layers == default
		? Physics.Raycast(this.rayProviderMB.Ray, out hit, float.MaxValue)
		: Physics.Raycast(this.rayProviderMB.Ray, out hit, float.MaxValue, this.layers);
}
