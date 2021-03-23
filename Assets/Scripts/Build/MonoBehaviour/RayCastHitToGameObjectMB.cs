using UnityEngine;

public class RayCastHitToGameObjectMB : BaseMorphMB<RaycastHit, GameObject>
{
	public override GameObject DoMorph(RaycastHit hit) => hit.transform.gameObject;
}
