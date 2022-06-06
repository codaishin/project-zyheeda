using Routines;
using UnityEngine;

public abstract class BaseRoutinePluginSO<TRoutinePlugin> :
	ScriptableObject,
	IPlugin
	where TRoutinePlugin :
		IPlugin,
		new()
{
	[SerializeField]
	protected TRoutinePlugin plugin = new TRoutinePlugin();

	public PluginFn GetPluginFnFor(GameObject agent) {
		return this.plugin.GetPluginFnFor(agent);
	}
}
