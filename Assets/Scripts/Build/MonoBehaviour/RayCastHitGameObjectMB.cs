using UnityEngine;

public class RayCastHitGameObjectMB : BaseRayCastHitMB<GameObject>
{
	public override bool Get(RaycastHit hit, out GameObject got) {
		got = hit.transform.gameObject;
		return true;
	}
}
