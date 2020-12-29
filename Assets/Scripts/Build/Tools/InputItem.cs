using System.Linq;
using UnityEngine;

public struct InputItem
{
	public delegate bool ValidationFunc(in KeyCode key, in KeyState state);

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
