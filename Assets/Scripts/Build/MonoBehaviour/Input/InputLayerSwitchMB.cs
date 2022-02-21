using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputLayerSwitchMB : MonoBehaviour
{
	public BaseInputConfigSO? inputConfig;
	public InputEnum.Map[] enable = new InputEnum.Map[0];
	public BaseChannelSO[] listenTo = new BaseChannelSO[0];


	private void Start() {
		foreach (BaseChannelSO e in this.listenTo) {
			e.AddListener(this.Set);
		}
	}

	private InputActionMap GetAction(InputEnum.Map item) {
		return this.inputConfig![item];
	}

	private void Set() {
		foreach (InputActionMap map in this.enable.Select(this.GetAction)) {
			map.Enable();
		}
	}
}
