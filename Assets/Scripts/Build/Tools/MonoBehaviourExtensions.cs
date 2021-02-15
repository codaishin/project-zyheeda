using UnityEngine;

public static class MonoBehaviourExtensions
{
	public static T RequireComponent<T>(this MonoBehaviour monoBehaviour)
		=> monoBehaviour.gameObject.RequireComponent<T>();
}
