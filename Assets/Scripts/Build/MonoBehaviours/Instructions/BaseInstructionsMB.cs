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

	public InstructionsFunc GetInstructionsFor(GameObject agent) {
		return this.instructions.GetInstructionsFor(agent);
	}
}
