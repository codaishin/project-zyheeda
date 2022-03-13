using UnityEngine;

[System.Serializable]
public struct Reference<T>
	where T : class
{
	[SerializeField] private Object? value;

	public T? Value {
		get {
			if (this.value == null) {
				return null;
			}
			if (this.value is T valid) {
				return valid;
			}
			throw new System.InvalidCastException(
				$"can't cast {this.value} to {typeof(T)}"
			);
		}
	}

	public static Reference<T> PointToComponent<TComponent>(
		TComponent component
	) where TComponent : Component, T {
		return new Reference<T> { value = component };
	}

	public static Reference<T> PointToScriptableObject<TScriptableObject>(
		TScriptableObject so
	) where TScriptableObject : ScriptableObject, T {
		return new Reference<T> { value = so };
	}
}
