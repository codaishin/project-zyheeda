using UnityEngine;

public abstract class BaseInstructionsSO<TInstructions> :
	ScriptableObject,
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
