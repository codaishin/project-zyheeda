using UnityEngine;

public class RayCastHitColliderMB : BaseRayCastHitMB<Collider>
{
	public override bool Get(RaycastHit hit, out Collider got)
	{
		got = hit.collider;
		return true;
	}
}
