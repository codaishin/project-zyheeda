using System;
using UnityEngine;

namespace Routines
{
	public class WeightData : Data
	{
		public float weight;
	}

	[Serializable]
	public class StateAnimationBlend :
		BaseModifierFactory<IAnimationStatesBlend, WeightData>
	{
		public Animation.BlendState blendState;

		public override IAnimationStatesBlend GetConcreteAgent(GameObject agent) {
			return agent.RequireComponent<IAnimationStatesBlend>(true);
		}

		public override WeightData GetRoutineData(Data data) {
			return data.As<WeightData>()!;
		}

		protected override Action? GetAction(
			IAnimationStatesBlend agent,
			WeightData data
		) {
			return () => agent.Blend(this.blendState, data.weight);
		}
	}
}
