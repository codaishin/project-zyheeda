using UnityEngine;

public class RayCastHitToColliderMB : BaseOnMorphMB<RaycastHit, Collider>
{
	public override bool TryMorph(RaycastHit hit, out Collider got)
	{
		got = hit.collider;
		return true;
	}
}
