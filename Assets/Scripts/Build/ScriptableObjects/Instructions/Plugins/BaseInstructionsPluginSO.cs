using System;
using UnityEngine;

public class PluginData
{
	public bool run;
	public float weight;
}

public struct PluginCallbacks
{
	public Action? onBegin;
	public Action? onUpdate;
	public Action? onEnd;
}

public abstract class BaseInstructionsPluginSO : ScriptableObject
{
	public abstract PluginCallbacks GetCallbacks(
		GameObject agent,
		PluginData data
	);
}
