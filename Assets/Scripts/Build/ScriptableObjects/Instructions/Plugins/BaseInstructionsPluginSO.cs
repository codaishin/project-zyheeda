using System;
using UnityEngine;

public abstract class BaseInstructionsPluginSO<TPluginData> : ScriptableObject
	where TPluginData : struct
{
	public abstract Action<TPluginData>? GetOnBegin(GameObject agent);
	public abstract Action<TPluginData>? GetOnEnd(GameObject agent);
}
