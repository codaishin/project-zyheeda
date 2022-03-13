using System;
using System.Linq;
using UnityEngine.InputSystem;

public class InputListenerMB : BaseListenerMB
{
	public BaseInputConfigSO? inputConfigSO;
	public InputEnum.Action[] listenTo = new InputEnum.Action[0];
	public Reference<IApplicable>[] apply = new Reference<IApplicable>[0];

	private InputAction? input;
	private Action? applyAll = null;
	private bool triggeredThisFrame = false;

	protected override void StartListening() {
		this.input!.performed += this.InvokeOnInput;
	}

	protected override void StopListening() {
		this.input!.performed -= this.InvokeOnInput;
	}

	private void InvokeOnInput(InputAction.CallbackContext _) {
		if (this.triggeredThisFrame || this.applyAll == null) {
			return;
		}
		this.applyAll();
		this.triggeredThisFrame = true;
	}

	protected override void Start() {
		this.input = this.inputConfigSO![this.listenTo[0]];
		this.applyAll = this.apply
			.Select(a => a.Value!)
			.Aggregate(this.applyAll, (l, c) => l + c.Apply);
		base.Start();
	}

	private void LateUpdate() {
		this.triggeredThisFrame = false;
	}
}
