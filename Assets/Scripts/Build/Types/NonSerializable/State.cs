using System;
using System.Collections.Generic;

public class State<TTransitionKey>
{
	public Dictionary<TTransitionKey, State<TTransitionKey>> Transitions { get; } =
		new Dictionary<TTransitionKey, State<TTransitionKey>>();

	public event Action onExit;
	public event Action onEnter;

	public bool TransitionTo(TTransitionKey key, out State<TTransitionKey> state)
	{
		if (this.Transitions.TryGetValue(key, out state)) {
			this.onExit?.Invoke();
			state.onEnter?.Invoke();
			return true;
		}
		return false;
	}
}
