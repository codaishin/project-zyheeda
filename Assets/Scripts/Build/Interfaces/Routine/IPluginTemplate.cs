using UnityEngine;

namespace Routines
{
	public interface IPlugin
	{
		PluginFn GetPluginFnFor(GameObject agent);
	}
}
