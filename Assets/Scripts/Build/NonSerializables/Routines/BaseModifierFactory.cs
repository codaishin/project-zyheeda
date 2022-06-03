using System;
using UnityEngine;

namespace Routines
{
	public abstract class BaseModifierFactory<TAgent, TData> : IModifierFactory
	{
		public abstract TAgent GetConcreteAgent(GameObject agent);
		public abstract TData GetRoutineData(Data data);

		protected abstract Action? GetAction(TAgent agent, TData data);

		public Routines.ModifierFn GetModifierFnFor(GameObject agent) {
			TAgent concreteAgent = this.GetConcreteAgent(agent);
			return data => this.GetAction(concreteAgent, this.GetRoutineData(data));
		}
	}
}
