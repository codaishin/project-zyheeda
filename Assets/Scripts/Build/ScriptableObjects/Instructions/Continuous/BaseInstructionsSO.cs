using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerable<YieldInstruction> CoroutineInstructions();

public abstract class BaseInstructionsSO : ScriptableObject
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	public abstract CoroutineInstructions InstructionsFor(GameObject agent);
}
