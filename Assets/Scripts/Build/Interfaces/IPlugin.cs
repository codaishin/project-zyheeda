using UnityEngine;

public interface IPlugin
{
	PluginHooksFn PluginHooksFor(GameObject agent);
}
