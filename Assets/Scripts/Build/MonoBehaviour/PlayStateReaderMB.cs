using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayStateReaderMB : MonoBehaviour
{
	private PlayState lastState;
	private Action<PlayState>? apply;

	public PlayStateSwitchSO? stateSwitch;
	public PlayState state;

	[Header("State Callbacks")]
	public UnityEvent? onStateEnter;
	public UnityEvent? onStateExit;

	private void Start() {
		if (this.stateSwitch == null) throw this.NullError();
		if (this.onStateEnter == null) this.onStateEnter = new UnityEvent();
		if (this.onStateExit == null) this.onStateExit = new UnityEvent();
		this.lastState = this.state;
		this.apply = this.ApplyFactory(this.onStateEnter, this.onStateExit);
		this.apply(this.stateSwitch.State);
		this.stateSwitch.OnStateChange += this.apply;
	}

	private void OnDestroy() {
		this.stateSwitch!.OnStateChange -= this.apply!;
	}

	private Action<PlayState> ApplyFactory(
		UnityEvent onStateEnter,
		UnityEvent onStateExit
	) => newState => {
		Maybe<UnityEvent> uEvent = this.GetEvent(
			newState,
			onStateEnter,
			onStateExit
		);
		uEvent.Match(some: e => e.Invoke(), none: () => { });
		this.lastState = newState;
	};

	private Maybe<UnityEvent> GetEvent(
		PlayState newState,
		UnityEvent onStateEnter,
		UnityEvent onStateExit
	) {
		switch (this.state) {
			case PlayState state when state == newState:
				return Maybe.Some(onStateEnter);
			case PlayState state when state == this.lastState:
				return Maybe.Some(onStateExit);
			default:
				return Maybe.None<UnityEvent>();
		}
	}
}
