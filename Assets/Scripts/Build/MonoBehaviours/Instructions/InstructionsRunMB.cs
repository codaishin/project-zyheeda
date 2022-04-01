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

	private Action<IInstructions> Delay(Action<IInstructions> action) {
		IEnumerator<YieldInstruction> delay(IInstructions source) {
			yield return new WaitForEndOfFrame();
			action(source);
		}
		return source => this.StartCoroutine(delay(source));
	}

	private void NowOrDelay(Action<IInstructions> action, IInstructions source) {
		action = this.delayApply
			? this.Delay(action)
			: action;
		action(source);
	}

	private void ApplyNew(IInstructions source) {
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

	private void ReleaseIfCurrentlyRunning(IInstructions source) {
		if (this.currentSource != source) {
			return;
		}
		this.isRunning = false;
	}

	public void Apply(IInstructions source) {
		this.NowOrDelay(this.ApplyNew, source);
	}

	public void Release(IInstructions source) {
		this.NowOrDelay(this.ReleaseIfCurrentlyRunning, source);
	}
}
