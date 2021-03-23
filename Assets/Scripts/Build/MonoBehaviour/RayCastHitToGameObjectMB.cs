using UnityEngine;

public class RayCastHitToGameObjectMB : BaseOnMorphMB<RaycastHit, GameObject>
{
	public override bool TryMorph(RaycastHit hit, out GameObject got) {
		got = hit.transform.gameObject;
		return true;
	}
}
