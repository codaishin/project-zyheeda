using UnityEngine;

public class DestroyOnEnd : IPlugin
{
	public PluginHooksFn PluginHooksFor(GameObject agent) {
		return _ => new PluginHooks {
			onEnd = () => GameObject.Destroy(agent),
		};
	}
}
