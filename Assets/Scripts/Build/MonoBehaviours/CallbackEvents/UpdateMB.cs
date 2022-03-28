using UnityEngine;
using UnityEngine.Events;

public class UpdateMB : MonoBehaviour
{
	public UnityEvent? onUpdate;

	private void Start() {
		if (this.onUpdate == null) {
			this.onUpdate = new UnityEvent();
		}
	}

	private void Update() => this.onUpdate?.Invoke();
}
