using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class AlignForwardWithMovement : BasePlugin<Transform, Data>
	{
		public override Transform GetConcreteAgent(GameObject agent) {
			return agent.transform;
		}

		public override Data GetData(Data data) {
			return data;
		}

		protected
		override Action? GetAction(
			Transform agent,
			Data data
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
