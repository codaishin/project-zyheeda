using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Hitters/HitProjectileTarget")]
public class HitProjectileTargetSO : ScriptableObject, IHit
{
	public Func<T?> Try<T>(GameObject source) where T : Component {
		ProjectileMB projectile = source.RequireComponent<ProjectileMB>();
		return () => projectile.target!.GetComponent<T>();
	}

	public Func<Vector3?> TryPoint(GameObject source) {
		ProjectileMB projectile = source.RequireComponent<ProjectileMB>();
		return () => projectile.target!.position;
	}
}
