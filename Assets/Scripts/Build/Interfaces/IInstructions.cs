using System;
using UnityEngine;

public interface IInstructions
{
	InstructionsFunc GetInstructionsFor(
		GameObject agent,
		Func<bool>? run = null
	);
}
