using UnityEngine;

public class RayCastHitCharacterSheetMB : BaseRayCastHitMB<CharacterSheetMB>
{
	public override bool Get(RaycastHit hit, out CharacterSheetMB got)
	{
		return hit.transform.TryGetComponent(out got);
	}
}
