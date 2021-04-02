using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectRunner
{
	IStack this[EffectTag tag, ConditionStacking stacking] { get; }
}
