using UnityEngine;

public class InstructionsReleaseMB : MonoBehaviour, IApplicable
{
	public Reference<IApplicable<IInstructions>> runner;

	public Reference<IInstructions>[] instructions =
		new Reference<IInstructions>[0];

	public void Apply() {
		foreach (IInstructions value in this.instructions.Values()) {
			this.runner.Value!.Release(value);
		}
	}

	public void Release() { }
}
