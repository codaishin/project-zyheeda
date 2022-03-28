using System;
using UnityEngine;

public class BaseInstructionsMB<TInstructions> :
	MonoBehaviour,
	IInstructions
	where TInstructions :
		IInstructions,
		new()
{
	[SerializeField]
	private TInstructions instructions = new TInstructions();

	public TInstructions Instructions => this.instructions;

	public InstructionsFunc GetInstructionsFor(
		GameObject agent,
		Func<bool>? run = null
	) {
		return this.instructions.GetInstructionsFor(agent, run);
	}
}
