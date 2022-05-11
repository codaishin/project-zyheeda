using System;
using System.Linq;
using UnityEngine.InputSystem;

public class InputListenerMB : BaseListenerMB
{

	public BaseInputConfigSO? inputConfigSO;
	public bool onPerformed;
	public bool onCanceled;
	public InputEnum.Action listenTo;
	public Reference<IApplicable>[] apply = new Reference<IApplicable>[0];

	private InputAction? input;
	private Action? applyAll = null;

	protected override void StartListening() {
		this.input!.performed += this.OnPerformed;
		this.input!.canceled += this.OnCanceled;
	}

	protected override void StopListening() {
		this.input!.performed -= this.OnPerformed;
		this.input!.canceled -= this.OnCanceled;
	}

	private void OnPerformed(InputAction.CallbackContext _) {
		if (!this.onPerformed || this.applyAll is null) {
			return;
		}
		this.applyAll();
	}

	private void OnCanceled(InputAction.CallbackContext _) {
		if (this.onCanceled is false || this.applyAll is null) {
			return;
		}
		this.applyAll();
	}

	protected override void Start() {
		Action getApply(IApplicable applicable) => applicable.Apply;
		Action concat(Action? fst, Action snd) => fst + snd;

		this.input = this.inputConfigSO![this.listenTo];
		this.applyAll = this.apply
			.Values()
			.Select(getApply)
			.Aggregate(null as Action, concat);
		base.Start();
	}
}
