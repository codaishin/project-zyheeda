using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class InputListenerMB : BaseListenerMB
{
	public BaseInputConfigSO? inputConfigSO;
	public bool callRelease = false;
	public InputEnum.Action[] listenTo = new InputEnum.Action[0];
	public Reference<IApplicable>[] apply = new Reference<IApplicable>[0];

	private InputAction? input;
	private Action? applyAll = null;
	private Action? releaseAll = null;
	private bool triggeredThisFrame = false;

	protected override void StartListening() {
		this.input!.performed += this.InvokeOnInput;
		this.input!.canceled += this.InvokeOnRelease;
	}

	protected override void StopListening() {
		this.input!.performed -= this.InvokeOnInput;
		this.input!.canceled -= this.InvokeOnRelease;
	}

	private void InvokeOnInput(InputAction.CallbackContext _) {
		if (this.triggeredThisFrame || this.applyAll == null) {
			return;
		}
		this.applyAll();
		this.triggeredThisFrame = true;
	}

	private void InvokeOnRelease(InputAction.CallbackContext _) {
		if (!this.callRelease || this.releaseAll is null) {
			return;
		}
		this.releaseAll();
	}

	private static Action GetApply(IApplicable applicable) {
		return applicable.Apply;
	}

	private static Action GetRelease(IApplicable applicable) {
		return applicable.Release;
	}

	private static Action ConcatActions(Action? fst, Action snd) {
		return fst + snd;
	}

	protected override void Start() {
		this.input = this.inputConfigSO![this.listenTo[0]];

		IEnumerable<IApplicable> applyItems = this.apply.Select(a => a.Value!);
		this.applyAll = applyItems
			.Select(InputListenerMB.GetApply)
			.Aggregate(null as Action, InputListenerMB.ConcatActions);
		this.releaseAll = applyItems
			.Select(InputListenerMB.GetRelease)
			.Aggregate(null as Action, InputListenerMB.ConcatActions);
		base.Start();
	}

	private void LateUpdate() {
		this.triggeredThisFrame = false;
	}
}
