using UnityEngine;

public class ApplyInstructionsOnMB : MonoBehaviour, IApplicable
{
	public InstructionHandleMB? handle;

	public Reference<IApplicable<InstructionHandleMB>>[] apply
		= new Reference<IApplicable<InstructionHandleMB>>[0];

	public void Apply() {
		foreach (IApplicable<InstructionHandleMB> elem in apply.Values()) {
			elem.Apply(this.handle!);
		}
	}

	public void Release() {
		foreach (IApplicable<InstructionHandleMB> elem in apply.Values()) {
			elem.Release(this.handle!);
		}
	}
}
