using UnityEngine;

public enum KeyState { Hold = default, Down, Up }

[CreateAssetMenu(menuName = "ScriptableObjects/KeyInput")]
public class KeyInputSO: BaseKeyInputSO
{
	protected override bool Get(in KeyCode code) => Input.GetKey(code);
	protected override bool GetDown(in KeyCode code) => Input.GetKeyDown(code);
	protected override bool GetUp(in KeyCode code) => Input.GetKeyUp(code);
}
