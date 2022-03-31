using System;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsMB : MonoBehaviour, IInstructions
{
	private InstructionsFunc? instructionsFunc;

	public Reference<IInstructionsTemplate> template;
	public Reference agent;

	private void Start() {
		this.instructionsFunc = this.template.Value!.GetInstructionsFor(
			this.agent.GameObject
		);
	}

	public IEnumerator<YieldInstruction?>? GetInstructions(Func<bool> run) {
		return this.instructionsFunc
			!.Invoke(run)
			?.GetEnumerator();
	}
}
