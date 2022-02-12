using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputLayerSwitchMB : MonoBehaviour
{
	public ActionMap setTo;
	public PlayerInputConfigSO? playerInputConfig;
	public BaseChannelSO[] listenTo = new BaseChannelSO[0];


	private void Start() {
		foreach (BaseChannelSO e in this.listenTo) {
			e.AddListener(this.Set);
		}
	}

	private void Set() {
		InputActionMap map = this.setTo switch {
			ActionMap.Movement => this.playerInputConfig!.Config.Movement.Get(),
			ActionMap.Mouse => this.playerInputConfig!.Config.Mouse.Get(),
			_ => throw new ArgumentException(
				$"no map configured for {this.setTo}"
			),
		};
		map.Enable();
	}
}
