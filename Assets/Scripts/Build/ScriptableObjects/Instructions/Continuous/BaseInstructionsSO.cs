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

public abstract class BaseInstructionsSO<TAgent, TPluginData> :
	BaseInstructionsSO
	where TPluginData :
		struct
{
	public BaseInstructionsPluginSO<TPluginData>[] plugins =
		new BaseInstructionsPluginSO<TPluginData>[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract CoroutineInstructions Instructions(TAgent agent);
	protected abstract TPluginData GetPluginData(GameObject agent);

	public override CoroutineInstructions GetInstructionsFor(
		GameObject agent,
		Func<bool>? keepRunningCheck = null
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		Action<TPluginData>? onBegin = this.GetPluginBegin(agent);
		Action<TPluginData>? onEnd = this.GetPluginEnd(agent);
		CoroutineInstructions instructions = this.Instructions(concreteAgent);

		return () => BaseInstructionsSO<TAgent, TPluginData>.RunInstructions(
			instructions,
			onBegin,
			onEnd,
			keepRunningCheck
		);
	}

	private Action<TPluginData>? GetPluginEnd(GameObject agent) =>
		this.plugins
			.Select(plugin => plugin.GetOnEnd(agent))
			.Aggregate(null as Action<TPluginData>, (l, c) => l + c);

	private Action<TPluginData>? GetPluginBegin(GameObject agent) =>
		this.plugins
			.Select(plugin => plugin.GetOnBegin(agent))
			.Aggregate(null as Action<TPluginData>, (l, c) => l + c);

	private static IEnumerable<YieldInstruction> RunInstructions(
		CoroutineInstructions instructions,
		Action<TPluginData>? onBegin,
		Action<TPluginData>? onEnd,
		Func<bool>? keepRunningCheck
	) {
		IEnumerable<YieldInstruction> loop = keepRunningCheck is Func<bool> check
			? BaseInstructionsSO<TAgent, TPluginData>.Loop(instructions, check)
			: BaseInstructionsSO<TAgent, TPluginData>.Loop(instructions);

		onBegin?.Invoke(default);
		foreach (YieldInstruction hold in loop) {
			yield return hold;
		}
		onEnd?.Invoke(default);
	}

	private static IEnumerable<YieldInstruction> Loop(
		CoroutineInstructions instructions
	) {
		return instructions();
	}

	private static IEnumerable<YieldInstruction> Loop(
		CoroutineInstructions instructions,
		Func<bool> runningCheck
	) {
		using IEnumerator<YieldInstruction> it = instructions().GetEnumerator();
		while (it.MoveNext() && runningCheck()) {
			yield return it.Current;
		}
	}
}
