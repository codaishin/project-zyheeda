using System;
using UnityEngine;

namespace Routines
{
	public abstract class BasePlugin<TAgent, TData> : IPlugin
	{
		public abstract TAgent GetConcreteAgent(GameObject agent);
		public abstract TData GetData(Data data);

		protected abstract Action? GetAction(TAgent agent, TData data);

		public Routines.PluginFn GetPluginFnFor(GameObject agent) {
			TAgent concreteAgent = this.GetConcreteAgent(agent);
			return data => this.GetAction(concreteAgent, this.GetData(data));
		}
	}
}
