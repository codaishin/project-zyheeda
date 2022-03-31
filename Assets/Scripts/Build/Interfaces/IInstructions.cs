using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInstructions
{
	IEnumerator<YieldInstruction?>? GetInstructions(Func<bool> run);
}
