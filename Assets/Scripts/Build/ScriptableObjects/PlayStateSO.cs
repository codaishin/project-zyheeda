using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum PlayState { None = default, Play, Paused, Menu }

public class PlayStateSO : ScriptableObject
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
}
