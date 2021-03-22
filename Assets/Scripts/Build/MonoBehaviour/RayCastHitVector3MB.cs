using UnityEngine;

public class RayCastHitVector3MB : BaseRayCastHitMB<Vector3>
{
	public override Vector3 Get(RaycastHit hit) => hit.point;
}
