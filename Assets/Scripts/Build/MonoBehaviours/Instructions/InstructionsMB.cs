using UnityEngine;

public class InstructionsMB : MonoBehaviour, IInstructions
{
	private ExternalInstructionsFn? instructionsFunc;

	public Reference<IInstructionsTemplate> template;
	public Reference agent;

	private void Start() {
		GameObject agent = this.agent.GameObject;
		this.instructionsFunc = this.template.Value!.GetInstructionsFor(agent);
	}

	public InstructionData? GetInstructionData() {
		return this.instructionsFunc!.Invoke();
	}
}
