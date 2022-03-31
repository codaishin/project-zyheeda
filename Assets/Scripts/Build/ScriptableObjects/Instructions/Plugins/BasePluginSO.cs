using UnityEngine;

public abstract class BasePluginSO<TPlugin> :
	ScriptableObject,
	IPlugin
	where TPlugin :
		IPlugin,
		new()
{
	[SerializeField]
	protected TPlugin plugin = new TPlugin();

	public PartialPluginCallbacks GetCallbacks(GameObject agent) {
		return this.plugin.GetCallbacks(agent);
	}
}
