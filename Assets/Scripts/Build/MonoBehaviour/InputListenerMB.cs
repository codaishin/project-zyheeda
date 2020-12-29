using System.Linq;
using UnityEngine;

public class InputListenerMB : MonoBehaviour
{
	public KeyCode keyCode;
	public KeyState keyState;
	public BaseInputSO inputSO;
	public EventSO[] events;

	public void Listen()
	{
		if (this.inputSO.GetKey(this.keyCode, this.keyState)) {
			this.events.ForEach(e => e.Raise());
		}
	}
}
