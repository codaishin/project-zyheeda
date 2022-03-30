using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplyInstructionsMB :
	MonoBehaviour,
	IApplicable<InstructionHandleMB>
{
	private InstructionsFunc? instructionsFunc;

	public Reference<IInstructions> instructions;
	public Reference agent;

	private void Start() {
		this.instructionsFunc = this.instructions.Value!.GetInstructionsFor(
			this.agent.GameObject
		);
	}

	private IEnumerator<YieldInstruction?>? GetCoroutine(Func<bool> run) {
		IEnumerable<YieldInstruction?>? routine = this.instructionsFunc!(run);
		if (routine == null) {
			return null;
		}
		return routine.GetEnumerator();
	}

	public void Apply(InstructionHandleMB runner) {
		IEnumerator<YieldInstruction?>? newRoutine = this.GetCoroutine(
			() => runner.IsRunning
		);

		if (newRoutine == null) {
			return;
		}

		runner.StopAllCoroutines();
		runner.IsRunning = true;
		runner.CurrentSource = this;
		runner.StartCoroutine(newRoutine);
	}

	public void Release(InstructionHandleMB runner) {
		if (runner.CurrentSource != this) {
			return;
		}
		runner.IsRunning = false;
	}
}
