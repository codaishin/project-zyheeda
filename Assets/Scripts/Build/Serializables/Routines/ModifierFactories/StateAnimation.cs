using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class StateAnimation : BaseModifierFactory<IAnimationStates, RoutineData>
	{
		public Animation.State state;

		public override IAnimationStates GetConcreteAgent(GameObject agent) {
			return agent.RequireComponent<IAnimationStates>(true);
		}

		public override RoutineData GetRoutineData(RoutineData data) {
			return data;
		}

		protected override Action? GetAction(IAnimationStates agent, RoutineData data) {
			return () => agent.Set(this.state);
		}
	}
}
