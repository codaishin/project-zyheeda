using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Hitters/Source")]
public class HitSourceSO : BaseHitSO
{
	public override T? Try<T>(T source) where T : class {
		return source;
	}

	public override Vector3? TryPoint(Transform source) {
		return source.position;
	}
}
