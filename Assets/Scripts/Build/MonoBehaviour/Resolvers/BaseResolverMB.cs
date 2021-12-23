using UnityEngine;
using UnityEngine.Events;

public abstract class BaseResolverMB<TComponent> :
	MonoBehaviour
	where TComponent :
		Component
{
	public UnityEvent<TComponent> onResolved = new UnityEvent<TComponent>();

	public void Resolve(GameObject target) {
		foreach (TComponent component in target.GetComponents<TComponent>()) {
			this.onResolved.Invoke(component);
		}
	}
}
