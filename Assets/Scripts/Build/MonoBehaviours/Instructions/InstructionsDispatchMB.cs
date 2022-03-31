using UnityEngine;

public class InstructionsDispatchMB : MonoBehaviour, IApplicable
{
	public Reference<IApplicable<IInstructions>> runner;
	public Reference<IInstructions> instructions;

	public void Apply() {
		this.runner.Value!.Apply(this.instructions.Value!);
	}

	public void Release() {
		this.runner.Value!.Release(this.instructions.Value!);
	}
}
