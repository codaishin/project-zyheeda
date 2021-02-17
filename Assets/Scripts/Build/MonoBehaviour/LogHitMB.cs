using UnityEngine;

public class LogHitMB : MonoBehaviour, IHitable, IGetGameObject
{
	public bool TryHit(in Attributes attributes)
	{
		Debug.Log($"Hit with {attributes}");
		return true;
	}
}
