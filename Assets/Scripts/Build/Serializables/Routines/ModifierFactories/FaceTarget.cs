using System;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class FaceTarget : BaseModifierFactory<Transform, TargetData>
	{
		public override Transform GetConcreteAgent(GameObject agent) {
			return agent.transform;
		}

		public override TargetData GetRoutineData(Data data) {
			return data.Extent<TargetData>();
		}

		protected override Action? GetAction(Transform agent, TargetData data) {
			return FaceTarget.LookAtTarget(agent, data);
		}

		private static Action LookAtTarget(Transform agent, TargetData data) {
			return () => {
				var target = data.target;

				if (target == null) {
					return;
				}

				var targetOnAgentHeight = new Vector3(
					target.position.x,
					agent.position.y,
					target.position.z
				);
				agent.transform.LookAt(targetOnAgentHeight);
			};
		}
	}
}
