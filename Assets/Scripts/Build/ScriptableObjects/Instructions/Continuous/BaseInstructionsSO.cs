using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate IEnumerable<YieldInstruction> CoroutineInstructions();

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract CoroutineInstructions GetInstructionsFor(
		GameObject agent,
		Func<bool>? keepRunningCheck = null
	);
}

public abstract class BaseInstructionsSO<TAgent> : BaseInstructionsSO
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract CoroutineInstructions Instructions(TAgent agent);

	public override CoroutineInstructions GetInstructionsFor(
		GameObject agent,
		Func<bool>? keepRunningCheck = null
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		Action? onBegin = this.GetPluginBegin(agent);
		Action? onEnd = this.GetPluginEnd(agent);
		CoroutineInstructions instructions = this.Instructions(concreteAgent);

		return () => BaseInstructionsSO<TAgent>.RunInstructions(
			instructions,
			onBegin,
			onEnd,
			keepRunningCheck
		);
	}

	private Action? GetPluginEnd(GameObject agent) =>
		this.plugins
			.Select(plugin => plugin.GetOnEnd(agent))
			.Aggregate(null as Action, (l, c) => l + c);

	private Action? GetPluginBegin(GameObject agent) =>
		this.plugins
			.Select(plugin => plugin.GetOnBegin(agent))
			.Aggregate(null as Action, (l, c) => l + c);

	private static IEnumerable<YieldInstruction> RunInstructions(
		CoroutineInstructions instructions,
		Action? onBegin,
		Action? onEnd,
		Func<bool>? keepRunningCheck
	) {
		IEnumerable<YieldInstruction> loop =
			keepRunningCheck is null
				? BaseInstructionsSO<TAgent>.Loop(instructions)
				: BaseInstructionsSO<TAgent>.Loop(instructions, keepRunningCheck);

		onBegin?.Invoke();
		foreach (YieldInstruction hold in loop) {
			yield return hold;
		}
		onEnd?.Invoke();
	}

	private static IEnumerable<YieldInstruction> Loop(
		CoroutineInstructions instructions
	) {
		return instructions();
	}

	private static IEnumerable<YieldInstruction> Loop(
		CoroutineInstructions instructions,
		Func<bool> keepRunningCheck
	) {
		using IEnumerator<YieldInstruction> it = instructions().GetEnumerator();
		while (it.MoveNext() && keepRunningCheck()) {
			yield return it.Current;
		}
	}
}
