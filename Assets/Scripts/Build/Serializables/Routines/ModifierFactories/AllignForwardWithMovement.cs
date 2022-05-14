using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class AllignForwardWithMovement : BaseModifierFactory<Transform, RoutineData>
	{
		public override Transform GetConcreteAgent(GameObject agent) {
			return agent.transform;
		}

		public override RoutineData GetRoutineData(RoutineData data) {
			return data;
		}

		protected
		override Action? GetAction(
			Transform agent,
			RoutineData data
		) {
			var lastPosition = agent.transform.position;

			void trackPosition() {
				lastPosition = agent.position;
			}
			void setDirection() {
				if (agent.position == lastPosition) {
					return;
				}
				agent.forward = agent.position - lastPosition;
			};

			return (Action)setDirection + trackPosition;
		}
	}
}
