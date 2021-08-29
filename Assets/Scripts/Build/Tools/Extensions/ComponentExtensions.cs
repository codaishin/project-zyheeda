using UnityEngine;

public static class ComponentExtensions
{
	public static T RequireComponent<T>(this Component monoBehaviour)
		=> monoBehaviour.gameObject.RequireComponent<T>();
}
