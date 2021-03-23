using UnityEngine;

public class RayCastHitToColliderMB : BaseMorphMB<RaycastHit, Collider>
{
	public override Collider DoMorph(RaycastHit hit) => hit.collider;
}
