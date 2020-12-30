using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseRayProviderMB : MonoBehaviour
{
	public abstract Ray Ray { get; }
}

[Serializable]
public class GameObjectEvent : UnityEvent<GameObject> {}

public class RayCastHitMB : MonoBehaviour
{
	public BaseRayProviderMB rayProviderMB;
	public GameObjectEvent onHitGameObject;

	private void Start()
	{
		if (this.onHitGameObject == null) {
			this.onHitGameObject = new GameObjectEvent();
		}
	}

	public void TryHit()
	{
		if (Physics.Raycast(this.rayProviderMB.Ray, out RaycastHit hit, float.MaxValue)) {
			this.onHitGameObject.Invoke(hit.transform.gameObject);
		}
	}
}
