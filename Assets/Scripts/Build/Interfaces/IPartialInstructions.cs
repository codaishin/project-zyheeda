using UnityEngine;

public interface IInstructionsTemplate
{
	InstructionsFunc GetInstructionsFor(GameObject agent);
}
