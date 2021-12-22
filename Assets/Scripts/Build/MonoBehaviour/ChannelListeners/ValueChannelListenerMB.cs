using UnityEngine;
using UnityEngine.Events;

public class ValueChannelListenerMB<T> : MonoBehaviour
{
	public ValueChannelSO<T>? listenTo;
	public UnityEvent<T> onRaise = new UnityEvent<T>();

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

	private void StartListening() {
		this.listenTo!.Listeners += this.onRaise.Invoke;
	}

	private void StopListening() {
		this.listenTo!.Listeners -= this.onRaise.Invoke;
	}
}
