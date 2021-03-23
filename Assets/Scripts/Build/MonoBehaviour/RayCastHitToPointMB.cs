using UnityEngine;

public class RayCastHitToPointMB : BaseMorphMB<RaycastHit, Vector3>
{
	public override Vector3 DoMorph(RaycastHit hit) => hit.point;
}
