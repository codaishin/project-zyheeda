using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectCreator<TSheet>
	where TSheet : ISheet
{
	EffectTag EffectTag { get; }
	Effect Create(TSheet source, TSheet target);
}
