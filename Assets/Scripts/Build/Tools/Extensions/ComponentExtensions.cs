using UnityEngine;

public static class ComponentExtensions
{
	public static T RequireComponent<T>(
		this Component component,
		bool includeChildren = false
	) where T : class {
		return component.gameObject.RequireComponent<T>(includeChildren);
	}
}
