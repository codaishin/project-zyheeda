using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionMapSwitchMB : MonoBehaviour
{
	public BaseInputConfigSO? inputConfig;
	public InputEnum.Map[] enable = new InputEnum.Map[0];
	public InputEnum.Map[] disable = new InputEnum.Map[0];
	public BaseChannelSO[] listenTo = new BaseChannelSO[0];


	private void Start() {
		foreach (BaseChannelSO e in this.listenTo) {
			e.AddListener(this.Switch);
		}
	}

	private InputActionMap GetAction(InputEnum.Map item) {
		return this.inputConfig![item];
	}

	private void Switch() {
		foreach (InputActionMap map in this.enable.Select(this.GetAction)) {
			map.Enable();
		}
		foreach (InputActionMap map in this.disable.Select(this.GetAction)) {
			map.Disable();
		}
	}
}
