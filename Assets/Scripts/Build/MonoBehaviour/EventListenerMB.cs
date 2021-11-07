using UnityEngine;
using UnityEngine.Events;

public class EventListenerMB : MonoBehaviour
{
	private bool listening;

	public EventSO? listenTo;
	public UnityEvent? onRaise;

	private void Start() {
		if (this.listenTo == null) throw this.NullError();
		if (this.onRaise == null) {
			this.onRaise = new UnityEvent();
		}
		this.StartListening(this.listenTo, this.onRaise);
	}

	private void OnDisable() {
		if (this.listenTo != null) {
			this.listenTo.Listeners -= this.onRaise!.Invoke;
			this.listening = false;
		}
	}

	private void OnEnable() {
		if (this.onRaise != null && this.listenTo != null) {
			this.StartListening(this.listenTo, this.onRaise);
		}
	}

	private void StartListening(EventSO listenTo, UnityEvent onRaise) {
		if (!this.listening) {
			listenTo.Listeners += onRaise.Invoke;
			this.listening = true;
		}
	}
}
