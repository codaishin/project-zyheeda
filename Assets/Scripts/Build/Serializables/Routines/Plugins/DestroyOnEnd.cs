using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class DestroyOnEnd : IPlugin
	{
		public PluginFn GetPluginFnFor(GameObject agent) {
			return _ => () => GameObject.Destroy(agent);
		}
	}
}
