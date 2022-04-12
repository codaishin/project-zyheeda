using System;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsRunMB : MonoBehaviour, IApplicable<IInstructions>
{
	public bool delayApply = false;

	private bool isRunning;
	private IInstructions? currentSource;
	private (Coroutine coroutine, Action release)? currentlyRunning;

	private void ForceStopCurrentInstruction() {
		if (this.currentlyRunning == null) {
			return;
		}
		var (coroutine, _) = this.currentlyRunning.Value;
		this.StopCoroutine(coroutine);
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
		var instructionData = source.GetInstructionData();

		if (instructionData == null) {
			return;
		}

		var (instructions, release) = instructionData.Value;

		this.ForceStopCurrentInstruction();
		this.currentSource = source;
		this.currentlyRunning = (
			this.StartCoroutine(instructions.GetEnumerator()),
			release
		);
	}

	private void ReleaseIfCurrentlyRunning(IInstructions source) {
		if (this.currentSource != source || this.currentlyRunning == null) {
			return;
		}

		var (_, release) = this.currentlyRunning.Value;
		release();
	}

	public void Apply(IInstructions source) {
		this.NowOrDelay(this.ApplyNew, source);
	}

	public void Release(IInstructions source) {
		this.NowOrDelay(this.ReleaseIfCurrentlyRunning, source);
	}
}
