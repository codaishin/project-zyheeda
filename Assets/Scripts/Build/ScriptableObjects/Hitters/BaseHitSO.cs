using UnityEngine;

public abstract class BaseHitSO : ScriptableObject, IHit
{
	public abstract T? Try<T>(T source) where T : Component;
	public abstract Vector3? TryPoint(Transform source);
}
