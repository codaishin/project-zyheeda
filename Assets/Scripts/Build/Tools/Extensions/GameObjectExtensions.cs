using UnityEngine;

public static class GameObjectExtensions
{
	private static bool TryGetComponentInChild<T>(
		this GameObject gameObject,
		out T component
	) where T : class {
		component = gameObject.GetComponentInChildren<T>();
		return component != null;
	}

	public static T RequireComponent<T>(
		this GameObject gameObject,
		bool includeChildren = false
	) where T : class {
		if (gameObject.TryGetComponent(out T component)) {
			return component;
		}
		if (includeChildren && gameObject.TryGetComponentInChild(out component)) {
			return component;
		}
		throw new MissingComponentException(
			$"GameObject \"{gameObject.name}\" does not have a Component of type \"{typeof(T)}\""
		);
	}
}
