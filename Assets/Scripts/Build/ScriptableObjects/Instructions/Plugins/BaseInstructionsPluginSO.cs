using System;
using UnityEngine;

public struct PluginCallbacks
{
	public Action? onBegin;
	public Action? onBeforeYield;
	public Action? onAfterYield;
	public Action? onEnd;

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
	public abstract Func<PluginData, PluginCallbacks> GetCallbacks(
		GameObject agent
	);
}
