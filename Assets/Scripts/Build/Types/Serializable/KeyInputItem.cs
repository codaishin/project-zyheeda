using System;
using UnityEngine;

[Serializable]
public struct KeyInputItem
{
	public delegate bool ValidationFunc(in KeyCode key, in KeyState state);

	public string name;
	public KeyCode keyCode;
	public KeyState keyState;
	public EventSO eventSO;

	public void Apply(in ValidationFunc validate)
	{
		if (validate(this.keyCode, this.keyState)) {
			this.eventSO.Raise();
		}
	}
}
