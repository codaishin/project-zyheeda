using System;
using UnityEngine;

public interface IHit
{
	Func<T?> Try<T>(GameObject source) where T : Component;
	Func<Vector3?> TryPoint(GameObject source);
}
