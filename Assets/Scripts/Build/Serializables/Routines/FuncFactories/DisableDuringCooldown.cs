using System;
using System.Collections.Generic;
using UnityEngine;

namespace Routines
{
	[Serializable]
	public class DisableDuringCooldown : BaseFuncFactory<GameObject>
	{
		public float cooldown;

		protected override GameObject ConcreteAgent(GameObject agent) {
			return agent;
		}

		protected override SubRoutineFn[] SubRoutines(GameObject agent) {
			IEnumerable<YieldInstruction> disable(RoutineData _) {
				agent.SetActive(false);
				yield return new WaitForSeconds(this.cooldown);
				agent.SetActive(true);
			}
			return new SubRoutineFn[] { disable };
		}
	}
}
