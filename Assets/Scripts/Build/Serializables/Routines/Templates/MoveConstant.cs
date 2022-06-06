using System;
using System.Collections.Generic;
using UnityEngine;

namespace Routines
{
	public struct MovementData
	{
		public Transform transform;
		public Func<Vector3?> getTarget;
	}

	[Serializable]
	public class MoveConstant : BaseTemplate<MovementData>
	{
		public Reference<IHit> hitter;
		public float speed = 1;
		public float weight = 1;

		protected override MovementData ConcreteAgent(GameObject agent) {
			return new MovementData {
				transform = agent.transform,
				getTarget = this.hitter.Value!.TryPoint(agent),
			};
		}

		protected override SubRoutineFn[] SubRoutines(MovementData agent) {
			return new SubRoutineFn[] { data => this.Move(agent, data) };
		}

		protected override void ExtendData(Data data) {
			data.Extent<WeightData>();
		}

		private IEnumerable<WaitForEndOfFrame> Move(
			MovementData agent,
			Data data
		) {
			Vector3? target = agent.getTarget();
			Transform transform = agent.transform;

			data.As<WeightData>()!.weight = this.weight;
			if (!target.HasValue) {
				yield break;
			}
			while (transform.position != target.Value) {
				yield return new WaitForEndOfFrame();
				transform.position = this.MoveTowards(transform.position, target.Value);
			}
		}

		private Vector3 MoveTowards(Vector3 current, Vector3 target) {
			return Vector3.MoveTowards(current, target, Time.deltaTime * this.speed);
		}
	}
}
