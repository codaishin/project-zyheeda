using UnityEngine;

public interface IInstructionsTemplate
{
	ExternalInstructionsFn GetInstructionsFor(GameObject agent);
}
