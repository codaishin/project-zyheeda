using UnityEngine;

public interface IHit
{
	Maybe<T> Try<T>(T source) where T : Component;

	Maybe<Vector3> TryPoint(Transform source);
}
