using System;
using UnityEngine;

namespace Routines
{
	public static class Modifiers
	{

		public static (Action? begin, Action? update, Action? end) Concat(
			(Action? begin, Action? update, Action? end) fst,
			(Action? begin, Action? update, Action? end) snd
		) {
			return (
				fst.begin + snd.begin,
				fst.update + snd.update,
				fst.end + snd.end
			);
		}
	}

	public abstract class BaseModifierFactory<TAgent, TData> : IModifierFactory
	{
		public abstract TAgent GetConcreteAgent(GameObject agent);
		public abstract TData GetRoutineData(Data data);

		protected abstract (Action? begin, Action? update, Action? end) GetModifiers(
			TAgent agent,
			TData data
		);

		public Routines.ModifierFn GetModifierFnFor(GameObject agent) {
			TAgent concreteAgent = this.GetConcreteAgent(agent);
			return data => {
				TData modifierData = this.GetRoutineData(data);
				return this.GetModifiers(concreteAgent, modifierData);
			};
		}
	}
}
