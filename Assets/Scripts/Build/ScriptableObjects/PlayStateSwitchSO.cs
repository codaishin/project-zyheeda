using System;
using UnityEngine;

public enum PlayState { None = default, Play, Paused, Menu }

[CreateAssetMenu(menuName = "ScriptableObjects/PlayState/Switch")]
public class PlayStateSwitchSO : ScriptableObject
{
	private PlayState state;

	public PlayState State {
		get => this.state;
		set => this.ApplyState(value);
	}

	public event Action<PlayState> OnStateChange;

	private void ApplyState(in PlayState state)
	{
		this.state = state;
		this.OnStateChange?.Invoke(state);
	}

	public void SetState(PlayStateValueSO value) => this.State = value.state;
}
