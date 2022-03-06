using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate IEnumerable<YieldInstruction> CoroutineInstructions();

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract CoroutineInstructions GetInstructionsFor(GameObject agent);
}

public abstract class BaseInstructionsSO<TAgent> : BaseInstructionsSO
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract CoroutineInstructions Instructions(TAgent agent);

	public override CoroutineInstructions GetInstructionsFor(GameObject agent) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		Action? onBegin = this.GetPluginBegin(agent);
		Action? onEnd = this.GetPluginEnd(agent);
		CoroutineInstructions instructions = this.Instructions(concreteAgent);

		return () => this.RunInstructions(instructions, onBegin, onEnd);
	}

	private Action? GetPluginEnd(GameObject agent) =>
		this.plugins
			.Select(plugin => plugin.GetOnEnd(agent))
			.Aggregate(null as Action, (l, c) => l + c);

	private Action? GetPluginBegin(GameObject agent) =>
		this.plugins
			.Select(plugin => plugin.GetOnBegin(agent))
			.Aggregate(null as Action, (l, c) => l + c);

	private IEnumerable<YieldInstruction> RunInstructions(
		CoroutineInstructions instructions,
		Action? onBegin,
		Action? onEnd
	) {
		onBegin?.Invoke();
		foreach (YieldInstruction hold in instructions()) {
			yield return hold;
		}
		onEnd?.Invoke();
	}
}
