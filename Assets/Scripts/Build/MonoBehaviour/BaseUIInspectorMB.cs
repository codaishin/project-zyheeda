using UnityEngine;

public abstract class BaseUIInspectorMB<T> : MonoBehaviour
{
	public abstract void Set(T value);
}
