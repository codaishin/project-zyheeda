using UnityEngine;

public abstract class BaseChannelListenerMB : MonoBehaviour
{
	public abstract void StartListening();
	public abstract void StopListening();

	private bool started;

	private void Start() {
		this.started = true;
		this.StartListening();
	}

	private void OnDisable() {
		this.StopListening();
	}

	private void OnEnable() {
		if (this.started) {
			this.StartListening();
		}
	}
}
