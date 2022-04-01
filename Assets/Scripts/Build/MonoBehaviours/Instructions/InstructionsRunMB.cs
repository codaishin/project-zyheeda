using System;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsRunMB : MonoBehaviour, IApplicable<IInstructions>
{
	public bool delayApply = false;

	private bool isRunning;
	private IInstructions? currentSource;
	private Coroutine? currentlyRunning;

	private void ForceStopCurrentInstruction() {
		if (this.currentlyRunning == null) {
			return;
		}
		this.StopCoroutine(this.currentlyRunning);
	}

	private void DelaydApply(IInstructions source) {
		IEnumerator<YieldInstruction> delay() {
			yield return new WaitForEndOfFrame();
			this.ImmediateApply(source);
		}
		this.StartCoroutine(delay());
	}

	private void ImmediateApply(IInstructions source) {
		IEnumerator<YieldInstruction?>? instructions =
			source.GetInstructions(() => this.isRunning);

		if (instructions == null) {
			return;
		}

		this.ForceStopCurrentInstruction();
		this.isRunning = true;
		this.currentSource = source;
		this.currentlyRunning = this.StartCoroutine(instructions);
	}

	public void Apply(IInstructions source) {
		Action apply = this.delayApply
			? () => this.DelaydApply(source)
			: () => this.ImmediateApply(source);
		apply();
	}

	public void Release(IInstructions source) {
		if (this.currentSource != source) {
			return;
		}
		this.isRunning = false;
	}
}
