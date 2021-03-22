using UnityEngine;

public class RayCastHitVector3MB : BaseRayCastHitMB<Vector3>
{
	public override bool Get(RaycastHit hit, out Vector3 got) {
		got = hit.point;
		return true;
	}
}
