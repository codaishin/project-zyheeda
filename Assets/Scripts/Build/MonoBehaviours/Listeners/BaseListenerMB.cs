using UnityEngine;

public abstract class BaseListenerMB : MonoBehaviour
{
	protected abstract void StartListening();
	protected abstract void StopListening();

	private bool started;

	protected virtual void Start() {
		this.started = true;
		this.StartListening();
	}

	protected virtual void OnDisable() {
		this.StopListening();
	}

	protected virtual void OnEnable() {
		if (this.started) {
			this.StartListening();
		}
	}
}
