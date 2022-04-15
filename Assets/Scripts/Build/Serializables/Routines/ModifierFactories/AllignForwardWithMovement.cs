using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class AllignForwardWithMovement : BaseModifierFactory<Transform, Data>
	{
		public override Transform GetConcreteAgent(GameObject agent) {
			return agent.transform;
		}

		public override Data GetRoutineData(Data data) {
			return data;
		}

		protected
		override (Action? begin, Action? update, Action? end) GetModifiers(
			Transform agent,
			Data data
		) {
			Vector3 lastPosition = agent.transform.position;
			Action trackPosition = () => lastPosition = agent.position;
			Action setDirection = () => {
				if (agent.position == lastPosition) {
					return;
				}
				agent.forward = agent.position - lastPosition;
			};
			return (
				begin: null,
				update: setDirection + trackPosition,
				end: null
			);
		}
	}
}
