using System;
using UnityEngine;

public abstract class BaseKeyInputSO : ScriptableObject
{
	protected abstract bool Get(KeyCode code);
	protected abstract bool GetDown(KeyCode code);
	protected abstract bool GetUp(KeyCode code);

	public bool GetKey(KeyCode code, KeyState state)
		=> state switch {
			KeyState.Up => this.GetUp(code),
			KeyState.Down => this.GetDown(code),
			KeyState.Hold => this.Get(code),
			_ => throw new ArgumentException($"KeyState \"{state}\" not recognised"),
		};
}
