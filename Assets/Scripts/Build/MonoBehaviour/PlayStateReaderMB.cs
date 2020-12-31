using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayStateReaderMB : MonoBehaviour
{
	public PlayStateSwitchSO stateSwitch;
	public PlayState state;

	public UnityEvent onStateEnter;

	private void Start()
	{
		if (this.onStateEnter == null) {
			this.onStateEnter = new UnityEvent();
		}
		this.stateSwitch.OnStateChange += this.Apply;
	}

	private void OnDestroy()
	{
		this.stateSwitch.OnStateChange -= this.Apply;
	}

	private void Apply(PlayState state)
	{
		if (state == this.state) {
			this.onStateEnter.Invoke();
		}
	}
}
