using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputListenerMB : MonoBehaviour
{
	public BaseInputConfigSO? inputConfigSO;
	public InputEnum.Action[] listenTo = new InputEnum.Action[0];
	public Reference<IApplicable>[] apply = new Reference<IApplicable>[0];

	private InputAction? input;
	private Action? applyAll = null;
	private bool triggeredThisFrame = false;
	private bool listening = false;

	private void StartListening() {
		if (this.input == null || this.listening) {
			return;
		}
		this.input.performed += this.InvokeOnInput;
		this.listening = true;
	}

	private void StopListening() {
		if (this.input == null || this.listening == false) {
			return;
		}
		this.input.performed -= this.InvokeOnInput;
		this.listening = false;
	}

	private void InvokeOnInput(InputAction.CallbackContext _) {
		if (this.triggeredThisFrame || this.applyAll == null) {
			return;
		}
		this.applyAll();
		this.triggeredThisFrame = true;
	}

	private void Start() {
		this.input = this.inputConfigSO![this.listenTo[0]];
		this.applyAll = this.apply
			.Select(a => a.Value!)
			.Aggregate(this.applyAll, (l, c) => l + c.Apply);
		this.StartListening();
	}

	private void OnDisable() {
		this.StopListening();
	}

	private void OnEnable() {
		this.StartListening();
	}

	private void LateUpdate() {
		this.triggeredThisFrame = false;
	}
}
