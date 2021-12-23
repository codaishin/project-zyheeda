using UnityEngine;

public interface IRayCastHit
{
	Maybe<RaycastHit> TryHit();
}
