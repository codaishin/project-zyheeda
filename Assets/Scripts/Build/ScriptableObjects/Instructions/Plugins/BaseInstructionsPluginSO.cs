using System;
using UnityEngine;

public class CorePluginData : PluginData
{
	public bool run;
	public float weight;
}

public struct PluginCallbacks
{
	public Action<CorePluginData>? onBegin;
	public Action<CorePluginData>? onBeforeYield;
	public Action<CorePluginData>? onAfterYield;
	public Action<CorePluginData>? onEnd;

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
	public virtual Func<PluginData, PluginData>? GetPluginDecorators() => null;
}
