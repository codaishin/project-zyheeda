using UnityEngine;

public abstract class BaseHitSO<THit> : ScriptableObject
	where THit : IHit, new()
{
	public THit hit = new THit();
}
