using System.Collections.Generic;
using UnityEngine;

namespace Routines
{
	public interface IRoutine : IEnumerable<YieldInstruction?>
	{
		bool NextSubRoutine();
	}
}
