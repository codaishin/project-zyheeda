using System;
using UnityEngine;

namespace Routines
{
	public struct MovementAnimators
	{
		public IAnimationStatesBlend statesBlend;
		public IAnimationStates states;
	}

	public class WeightData : Data
	{
		public float weight;
	}

	[Serializable]
	public class MovementAnimation :
		BaseModifierFactory<MovementAnimators, WeightData>
	{
		public override MovementAnimators GetConcreteAgent(GameObject agent) {
			return new MovementAnimators {
				statesBlend = agent.RequireComponent<IAnimationStatesBlend>(true),
				states = agent.RequireComponent<IAnimationStates>(true),
			};
		}

		public override WeightData GetRoutineData(Data data) {
			return data.As<WeightData>()!;
		}

		protected
		override (Action? begin, Action? update, Action? end) GetModifiers(
			MovementAnimators agent,
			WeightData data
		) {
			return (
				begin: () => {
					agent.states.Set(Animation.State.WalkOrRun);
					agent.statesBlend.Blend(Animation.BlendState.WalkOrRun, data.weight);
				},
				update: () => {
					agent.statesBlend.Blend(Animation.BlendState.WalkOrRun, data.weight);
				},
				end: () => {
					agent.states.Set(Animation.State.Idle);
				}
			);
		}
	}
}
