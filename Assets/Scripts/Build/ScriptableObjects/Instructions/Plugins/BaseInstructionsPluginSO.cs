using System;
using UnityEngine;

public struct PluginCallbacks
{
	public Action<PluginData>? onBegin;
	public Action<PluginData>? onBeforeYield;
	public Action<PluginData>? onAfterYield;
	public Action<PluginData>? onEnd;

	public static PluginCallbacks operator +(
		PluginCallbacks a,
		PluginCallbacks b
	) {
		return new PluginCallbacks {
			onBegin = a.onBegin + b.onBegin,
			onBeforeYield = a.onBeforeYield + b.onBeforeYield,
			onAfterYield = a.onAfterYield + b.onAfterYield,
			onEnd = a.onEnd + b.onEnd,
		};
	}
}

public abstract class BaseInstructionsPluginSO : ScriptableObject
{
	public abstract PluginCallbacks GetCallbacks(GameObject agent);
	public virtual void ExtendPluginData(PluginData data) { }
}
