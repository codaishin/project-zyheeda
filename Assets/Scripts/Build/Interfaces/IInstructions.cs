using UnityEngine;

public interface IInstructions
{
	InstructionsFunc GetInstructionsFor(GameObject agent);
}
