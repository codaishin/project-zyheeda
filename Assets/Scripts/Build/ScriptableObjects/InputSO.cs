using System;
using UnityEngine;

public enum KeyState { Hold = default, Down, Up }

public abstract class BaseInputSO : ScriptableObject
{
	protected abstract bool Get(in KeyCode code);
	protected abstract bool GetDown(in KeyCode code);
	protected abstract bool GetUp(in KeyCode code);

	public bool GetKey(in KeyCode code, in KeyState state) => state switch {
			KeyState.Up => this.GetUp(code),
			KeyState.Down => this.GetDown(code),
			KeyState.Hold => this.Get(code),
			_ => throw new ArgumentException($"KeyState \"{state}\" not recognised"),
		};
}

[CreateAssetMenu(menuName = "ScriptableObjects/Input")]
public class InputSO: BaseInputSO
{
	protected override bool Get(in KeyCode code) => Input.GetKey(code);
	protected override bool GetDown(in KeyCode code) => Input.GetKeyDown(code);
	protected override bool GetUp(in KeyCode code) => Input.GetKeyUp(code);
}
