using UnityEngine;

public static class GameObjectExtensions
{
	public static T RequireComponent<T>(this GameObject gameObject) {
		if (gameObject.TryGetComponent(out T component)) {
			return component;
		}
		throw new MissingComponentException(
			$"GameObject \"{gameObject.name}\" does not have a Component of type \"{typeof(T)}\""
		);
	}
}
