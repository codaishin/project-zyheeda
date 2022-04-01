using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DisableDuringCooldownTemplate : BaseInstructionsTemplate<GameObject>
{
	public float cooldown;

	protected override GameObject GetConcreteAgent(GameObject agent) {
		return agent;
	}

	protected override PartialInstructionFunc PartialInstructions(
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
