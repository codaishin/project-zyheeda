using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionMapSwitchMB : MonoBehaviour
{
	public BaseInputConfigSO? inputConfig;
	public InputEnum.Map[] enable = new InputEnum.Map[0];
	public InputEnum.Map[] disable = new InputEnum.Map[0];
	public Reference<IChannel>[] listenTo = new Reference<IChannel>[0];


	private void Start() {
		foreach (Reference<IChannel> channel in this.listenTo) {
			channel.Value!.AddListener(this.Switch);
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
