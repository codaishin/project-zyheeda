using System;
using System.Linq;
using UnityEngine.InputSystem;

public class InputListenerMB : BaseListenerMB
{

	public BaseInputConfigSO? inputConfigSO;
	public InputEnum.Action listenTo;
	public Reference<IApplicable>[] onPerformed = new Reference<IApplicable>[0];
	public Reference<IApplicable>[] onCanceled = new Reference<IApplicable>[0];

	private InputAction? input;
	private Action? onPerformedAll = null;
	private Action? onCanceledAll = null;

	protected override void StartListening() {
		this.input!.performed += this.OnPerformed;
		this.input!.canceled += this.OnCanceled;
	}

	protected override void StopListening() {
		this.input!.performed -= this.OnPerformed;
		this.input!.canceled -= this.OnCanceled;
	}

	private void OnPerformed(InputAction.CallbackContext _) {
		if (this.onPerformedAll is null) {
			return;
		}
		this.onPerformedAll();
	}

	private void OnCanceled(InputAction.CallbackContext _) {
		if (this.onCanceledAll is null) {
			return;
		}
		this.onCanceledAll();
	}

	protected override void Start() {
		Action getApply(IApplicable applicable) => applicable.Apply;
		Action concat(Action? fst, Action snd) => fst + snd;

		this.input = this.inputConfigSO![this.listenTo];
		this.onPerformedAll = this.onPerformed
			.Values()
			.Select(getApply)
			.Aggregate(null as Action, concat);
		this.onCanceledAll = this.onCanceled
			.Values()
			.Select(getApply)
			.Aggregate(null as Action, concat);
		base.Start();
	}
}
