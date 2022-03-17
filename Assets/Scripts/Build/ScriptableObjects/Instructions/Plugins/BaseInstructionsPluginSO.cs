using System;
using UnityEngine;

public class PluginData
{
	public float weight;
}

public abstract class BaseInstructionsPluginSO : ScriptableObject
{
	public abstract Action? GetOnBegin(GameObject agent, PluginData data);
	public abstract Action? GetOnUpdate(GameObject agent, PluginData data);
	public abstract Action? GetOnEnd(GameObject agent, PluginData data);
}
