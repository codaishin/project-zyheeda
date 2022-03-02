using UnityEngine;

public interface IHit
{
	T? Try<T>(T source) where T : Component;
	Vector3? TryPoint(Transform source);
}
