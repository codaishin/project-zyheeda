using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class StateAnimation : BasePlugin<IAnimationStates, Data>
	{
		public Animation.State state;

		public override IAnimationStates GetConcreteAgent(GameObject agent) {
			return agent.RequireComponent<IAnimationStates>(true);
		}

		public override Data GetData(Data data) {
			return data;
		}

		protected override Action? GetAction(IAnimationStates agent, Data data) {
			return () => agent.Set(this.state);
		}
	}
}
