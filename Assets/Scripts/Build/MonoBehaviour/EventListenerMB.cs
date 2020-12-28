using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventListenerMB : MonoBehaviour
{
	private bool listening;

	public EventSO listenTo;
	public UnityEvent onRaise;

	private void Start()
	{
		if (this.onRaise == null) {
			this.onRaise = new UnityEvent();
		}
		this.StartListening();
	}

	private void OnDisable()
	{
		if (this.listenTo) {
			this.listenTo.Listeners -= this.onRaise.Invoke;
			this.listening = false;
		}
	}

	private void OnEnable()
	{
		if (this.onRaise != null) {
			this.StartListening();
		}
	}

	private void StartListening()
	{
		if (!this.listening) {
			this.listenTo.Listeners += this.onRaise.Invoke;
			this.listening = true;
		}
	}
}
