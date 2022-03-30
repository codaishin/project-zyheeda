using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseApplicablesOnMB : MonoBehaviour, IApplicable
{
	public InstructionHandleMB? handle;

	public Reference<IApplicable<InstructionHandleMB>>[] instructions =
		new Reference<IApplicable<InstructionHandleMB>>[0];

	public void Apply() {
		this.instructions
			.Values()
			.ForEach(a => a.Release(this.handle!));
	}

	public void Release() { }
}
