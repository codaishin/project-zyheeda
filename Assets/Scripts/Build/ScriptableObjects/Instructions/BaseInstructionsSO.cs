using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerable<YieldInstruction> CoroutineInstructions();

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract CoroutineInstructions InstructionsFor(GameObject agent);
}
