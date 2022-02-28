using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Hitters/MousePosition")]
public class HitMousePositionSO : BaseHitSO
{
	public override Maybe<T> Try<T>(T source) {
		throw new System.NotImplementedException();
	}

	public override Maybe<Vector3> TryPoint(Transform source) {
		throw new System.NotImplementedException();
	}
}
