using System.Collections.Generic;
using UnityEngine;

public class InstructionsRunMB : MonoBehaviour, IApplicable<IInstructions>
{
	private bool isRunning;
	private IInstructions? currentSource;
	private Coroutine? currentlyRunning;

	private void ForceStopCurrentInstruction() {
		if (this.currentlyRunning == null) {
			return;
		}
		this.StopCoroutine(this.currentlyRunning);
	}

	public void Apply(IInstructions source) {
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

	public void Release(IInstructions source) {
		if (this.currentSource != source) {
			return;
		}
		this.isRunning = false;
	}
}
