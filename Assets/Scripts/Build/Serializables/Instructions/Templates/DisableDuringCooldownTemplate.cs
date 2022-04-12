using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DisableDuringCooldownTemplate : BaseInstructionsTemplate<GameObject>
{
	public float cooldown;

	protected override GameObject ConcreteAgent(GameObject agent) {
		return agent;
	}

	protected override InternalInstructionFn InternalInstructionsFn(
		GameObject agent
	) {
		IEnumerable<YieldInstruction> disable(PluginData _) {
			agent.SetActive(false);
			yield return new WaitForSeconds(this.cooldown);
			agent.SetActive(true);
		}
		return disable;
	}
}
