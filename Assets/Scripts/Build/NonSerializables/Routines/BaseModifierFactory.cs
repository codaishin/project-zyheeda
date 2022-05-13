using System;
using UnityEngine;

namespace Routines
{
	[Flags]
	public enum ModifierHook
	{
		OnBegin = 1,
		OnUpdate = 2,
		OnEnd = 4,
	}

	[Serializable]
	public struct ModifierData
	{
		public ModifierHook hook;
		public Reference<IModifierFactory> factory;
	}

	public abstract class BaseModifierFactory<TAgent, TData> : IModifierFactory
	{
		public abstract TAgent GetConcreteAgent(GameObject agent);
		public abstract TData GetRoutineData(RoutineData data);

		protected abstract Action? GetAction(TAgent agent, TData data);

		public Routines.ModifierFn GetModifierFnFor(GameObject agent) {
			TAgent concreteAgent = this.GetConcreteAgent(agent);
			return data => {
				TData modifierData = this.GetRoutineData(data);
				return this.GetAction(concreteAgent, modifierData);
			};
		}
	}
}
