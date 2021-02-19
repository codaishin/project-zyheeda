using UnityEngine;

public interface IMagazine
{
	Disposable<GameObject> GetOrMakeProjectile();
}
