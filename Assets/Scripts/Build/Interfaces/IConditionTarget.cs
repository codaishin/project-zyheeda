using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionTarget
{
	void Add(Effect effect, EffectTag tag, ConditionStacking stacking);
}
