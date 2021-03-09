using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionTarget<TStackIntensity, TStackDuration>
	where TStackIntensity : IConditionManager
	where TStackDuration : IConditionManager
{
	TStackIntensity StackIntensity { get; }
	TStackDuration StackDuration { get; }
}
