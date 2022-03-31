using UnityEngine;

public interface IPlugin
{
	PartialPluginCallbacks GetCallbacks(GameObject agent);
}
