using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayStateReaderMB : MonoBehaviour
{
	private PlayState lastState;

	public PlayStateSwitchSO stateSwitch;
	public PlayState state;

	[Header("State Callbacks")]
	public UnityEvent onStateEnter;
	public UnityEvent onStateExit;

	private void Start()
	{
		if (this.onStateEnter == null) {
			this.onStateEnter = new UnityEvent();
		}
		if (this.onStateExit == null) {
			this.onStateExit = new UnityEvent();
		}
		this.lastState = this.stateSwitch.State;
		this.stateSwitch.OnStateChange += this.Apply;
	}

	private void OnDestroy()
	{
		this.stateSwitch.OnStateChange -= this.Apply;
	}

	private void Apply(PlayState newState)
	{
		if (newState == this.state) {
			this.onStateEnter.Invoke();
		} else if (this.lastState == this.state) {
			this.onStateExit.Invoke();
		}
		this.lastState = newState;
	}
}
