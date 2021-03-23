using UnityEngine;

public class RayCastHitToPointMB : BaseOnMorphMB<RaycastHit, Vector3>
{
	public override bool TryMorph(RaycastHit hit, out Vector3 got) {
		got = hit.point;
		return true;
	}
}
