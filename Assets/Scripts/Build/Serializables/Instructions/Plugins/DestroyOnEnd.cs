using UnityEngine;

public class DestroyOnEnd : IPlugin
{
	public PartialPluginCallbacks GetCallbacks(GameObject agent) {
		return _ => new PluginCallbacks {
			onEnd = () => GameObject.Destroy(agent),
		};
	}
}
