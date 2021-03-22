using UnityEngine;

public class RayCastHitGameObjectMB : BaseRayCastHitMB<GameObject>
{
	public override GameObject Get(RaycastHit hit) => hit.transform.gameObject;
}
