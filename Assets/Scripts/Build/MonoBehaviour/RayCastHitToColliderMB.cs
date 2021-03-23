using UnityEngine;

public class RayCastHitToColliderMB : BaseConditionalMorphMB<RaycastHit, Collider>
{
	public override bool TryMorph(RaycastHit hit, out Collider got)
	{
		got = hit.collider;
		return true;
	}
}
