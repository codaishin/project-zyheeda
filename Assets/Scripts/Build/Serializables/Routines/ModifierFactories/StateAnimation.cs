using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class StateAnimation : BaseModifierFactory<IAnimationStates, Data>
	{
		public Animation.State beginState;
		public Animation.State endState;

		public override IAnimationStates GetConcreteAgent(GameObject agent) {
			return agent.RequireComponent<IAnimationStates>(true);
		}

		public override Data GetRoutineData(Data data) {
			return data;
		}

		protected
		override (Action? begin, Action? update, Action? end) GetModifiers(
			IAnimationStates agent,
			Data data
		) {
			return (
				begin: () => agent.Set(this.beginState),
				update: null,
				end: () => agent.Set(this.endState)
			);
		}
	}
}
