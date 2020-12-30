using System;
using UnityEngine;

public abstract class BaseInputSO : ScriptableObject
{
	protected abstract bool Get(in KeyCode code);
	protected abstract bool GetDown(in KeyCode code);
	protected abstract bool GetUp(in KeyCode code);

	public virtual bool GetKey(in KeyCode code, in KeyState state)
		=> state switch {
			KeyState.Up => this.GetUp(code),
			KeyState.Down => this.GetDown(code),
			KeyState.Hold => this.Get(code),
			_ => throw new ArgumentException($"KeyState \"{state}\" not recognised"),
		};
}
