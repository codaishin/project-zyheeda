using UnityEngine;

public class RayCastHitToCharacterSheetMB : BaseConditionalMorphMB<RaycastHit, CharacterSheetMB>
{
	public override bool TryMorph(RaycastHit seed, out CharacterSheetMB morph)
	{
		return seed.transform.TryGetComponent(out morph);
	}
}
