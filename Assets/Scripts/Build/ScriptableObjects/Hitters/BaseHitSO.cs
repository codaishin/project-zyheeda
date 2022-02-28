using UnityEngine;

public abstract class BaseHitSO : ScriptableObject, IHit
{
	public abstract Maybe<T> Try<T>(T source) where T : Component;
	public abstract Maybe<Vector3> TryPoint(Transform source);
}
