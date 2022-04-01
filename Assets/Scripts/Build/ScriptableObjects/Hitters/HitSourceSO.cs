using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Hitters/Source")]
public class HitSourceSO : ScriptableObject, IHit
{
	public Func<T?> Try<T>(GameObject source) where T : Component {
		T component = source.GetComponent<T>();
		return () => component;
	}

	public Func<Vector3?> TryPoint(GameObject source) {
		Transform transform = source.transform;
		return () => transform.position;
	}
}
