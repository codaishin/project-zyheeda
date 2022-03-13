using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionMapSwitchMB : MonoBehaviour, IApplicable
{
	public BaseInputConfigSO? inputConfig;
	public InputEnum.Map[] enable = new InputEnum.Map[0];
	public InputEnum.Map[] disable = new InputEnum.Map[0];

	private InputActionMap GetAction(InputEnum.Map item) {
		return this.inputConfig![item];
	}

	public void Apply() {
		foreach (InputActionMap map in this.enable.Select(this.GetAction)) {
			map.Enable();
		}
		foreach (InputActionMap map in this.disable.Select(this.GetAction)) {
			map.Disable();
		}
	}
}
