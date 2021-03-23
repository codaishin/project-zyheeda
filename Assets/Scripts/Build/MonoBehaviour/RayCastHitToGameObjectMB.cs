using UnityEngine;

public class RayCastHitToGameObjectMB : BaseConditionalMorphMB<RaycastHit, GameObject>
{
	public override bool TryMorph(RaycastHit hit, out GameObject got) {
		got = hit.transform.gameObject;
		return true;
	}
}
