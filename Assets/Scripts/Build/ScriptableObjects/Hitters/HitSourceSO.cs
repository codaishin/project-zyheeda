using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Hitters/Source")]
public class HitSourceSO : BaseHitSO
{
	public override Maybe<T> Try<T>(T source) {
		return Maybe.Some(source);
	}

	public override Maybe<Vector3> TryPoint(Transform source) {
		return Maybe.Some(source.position);
	}
}
