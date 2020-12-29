using System.Linq;
using UnityEngine;

public struct InputItem
{
	public KeyCode keyCode;
	public KeyState keyState;
	public BaseInputSO inputSO;
	public EventSO[] events;

	public void Apply()
	{
		if (this.inputSO.GetKey(this.keyCode, this.keyState)) {
			this.events.ForEach(e => e.Raise());
		}
	}
}
