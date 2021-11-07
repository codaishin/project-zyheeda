using UnityEngine;
using UnityEngine.Events;

public class LateUpdateMB : MonoBehaviour
{
	public UnityEvent? onLateUpdate;

	private void Start() {
		if (this.onLateUpdate == null) {
			this.onLateUpdate = new UnityEvent();
		}
	}

	private void LateUpdate() => this.onLateUpdate!.Invoke();
}
