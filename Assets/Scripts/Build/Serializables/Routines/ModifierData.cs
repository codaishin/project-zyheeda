using System;

namespace Routines
{
	[Serializable]
	public struct PluginData
	{
		public PluginFlags flag;
		public Reference<IPlugin> plugin;
	}
}
