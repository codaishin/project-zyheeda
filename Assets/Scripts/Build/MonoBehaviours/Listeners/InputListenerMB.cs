using System;
using System.Linq;
using UnityEngine.InputSystem;

public class InputListenerMB : BaseListenerMB
{
	public BaseInputConfigSO? inputConfigSO;
	public bool callRelease = false;
	public InputEnum.Action listenTo;
	public Reference<IApplicable>[] apply = new Reference<IApplicable>[0];

	private InputAction? input;
	private Action? applyAll = null;
	private Action? releaseAll = null;

	protected override void StartListening() {
		this.input!.performed += this.InvokeOnInput;
		this.input!.canceled += this.InvokeOnRelease;
	}

	protected override void StopListening() {
		this.input!.performed -= this.InvokeOnInput;
		this.input!.canceled -= this.InvokeOnRelease;
	}

	private void InvokeOnInput(InputAction.CallbackContext _) {
		if (this.applyAll is null) {
			return;
		}
		this.applyAll();
	}

	private void InvokeOnRelease(InputAction.CallbackContext _) {
		if (this.callRelease is false || this.releaseAll is null) {
			return;
		}
		this.releaseAll();
	}

	protected override void Start() {
		Action getApply(IApplicable applicable) => applicable.Apply;
		Action getRelease(IApplicable applicable) => applicable.Release;
		Action concat(Action? fst, Action snd) => fst + snd;

		this.input = this.inputConfigSO![this.listenTo];
		this.applyAll = this.apply
			.Values()
			.Select(getApply)
			.Aggregate(null as Action, concat);
		this.releaseAll = this.apply
			.Values()
			.Select(getRelease)
			.Aggregate(null as Action, concat);
		base.Start();
	}
}
