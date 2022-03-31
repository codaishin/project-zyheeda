using UnityEngine;

public class BaseInstructionsMB<TInstructions> :
	MonoBehaviour,
	IInstructionsTemplate
	where TInstructions :
		IInstructionsTemplate,
		new()
{
	[SerializeField]
	private TInstructions instructions = new TInstructions();

	public TInstructions Instructions => this.instructions;

	public InstructionsFunc GetInstructionsFor(GameObject agent) {
		return this.instructions.GetInstructionsFor(agent);
	}
}
