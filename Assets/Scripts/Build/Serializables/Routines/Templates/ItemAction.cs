using System;
using System.Collections.Generic;
using UnityEngine;

namespace Routines
{
	public class TargetData : Data
	{
		public Transform? target;
	}

	public struct ItemActionData
	{
		public Transform transform;
		public Func<Transform?> getTarget;
		public IApplicable<Transform> effect;
	}


	[Serializable]
	public class ItemAction : BaseTemplate<ItemActionData>
	{
		public float preCastSeconds;
		public float afterCastSeconds;
		public Reference<IHit> hitter;
		public Reference<IApplicable<Transform>> effect;

		protected override void ExtendData(Data pluginData) {
			pluginData.Extent<TargetData>();
		}

		protected override ItemActionData ConcreteAgent(GameObject agent) {
			if (this.hitter.Value == null) {
				throw new NullReferenceException(
					$"hitter on {this} must be set, but was null"
				);
			}
			if (this.effect.Value == null) {
				throw new NullReferenceException(
					$"effect on {this} must be set, but was null"
				);
			}
			return new ItemActionData {
				transform = agent.transform,
				getTarget = this.hitter.Value!.Try<Transform>(agent),
				effect = this.effect.Value,
			};
		}

		protected override SubRoutineFn[] SubRoutines(ItemActionData agent) {
			IEnumerable<YieldInstruction>? action(Data pluginData) {
				Transform? target = agent.getTarget();

				if (target == null) {
					return null;
				}

				pluginData.As<TargetData>()!.target = target;

				return this.Instructions(agent, target);
			}

			return new SubRoutineFn[] { action };
		}

		private IEnumerable<YieldInstruction> Instructions(
			ItemActionData agent,
			Transform target
		) {
			Func<bool> notElapsed;

			notElapsed = ItemAction.NotElapsed(this.preCastSeconds);
			while (notElapsed()) {
				yield return new WaitForEndOfFrame();
			}

			agent.effect.Apply(target);

			notElapsed = ItemAction.NotElapsed(this.afterCastSeconds);
			while (notElapsed()) {
				yield return new WaitForEndOfFrame();
			}
		}

		private static Func<bool> NotElapsed(float time) {
			float elapsed = 0;
			return () => {
				bool notElapsed = elapsed < time;
				elapsed += Time.deltaTime;
				return notElapsed;
			};
		}
	}
}
