using System;
using UnityEngine;

public class EventSO : ScriptableObject
{
	public event Action Callbacks;

	public void Raise() => this.Callbacks?.Invoke();
}
