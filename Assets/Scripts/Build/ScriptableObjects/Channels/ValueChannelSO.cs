using System;
using UnityEngine;

public class ValueChannelSO<T> : ScriptableObject
{
	public event Action<T>? Listeners;

	public void Raise(T value) => this.Listeners?.Invoke(value);
}
