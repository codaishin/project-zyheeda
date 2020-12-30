using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseRayProviderMB : MonoBehaviour
{
	public abstract Ray Ray { get; }
}

[Serializable]
public class GameObjectEvent : UnityEvent<GameObject> {}

[Serializable]
public class Vector3Event : UnityEvent<Vector3> {}

public class RayCastHitMB : MonoBehaviour
{
	public BaseRayProviderMB rayProviderMB;
	public GameObjectEvent onHitGameObject;
	public Vector3Event onHitVector3;

	private void Start()
	{
		if (this.onHitGameObject == null) {
			this.onHitGameObject = new GameObjectEvent();
		}
		if (this.onHitVector3 == null) {
			this.onHitVector3 = new Vector3Event();
		}
	}

	public void TryHit()
	{
		if (Physics.Raycast(this.rayProviderMB.Ray, out RaycastHit hit, float.MaxValue)) {
			this.onHitGameObject.Invoke(hit.transform.gameObject);
			this.onHitVector3.Invoke(hit.point);
		}
	}
}
