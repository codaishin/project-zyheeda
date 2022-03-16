using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract Func<IEnumerable<YieldInstruction>> GetInstructionsFor(
		GameObject agent,
		Func<bool>? keepRunningCheck = null
	);
}

public delegate IEnumerable<YieldInstruction> CoroutineInstructions();

public abstract class BaseInstructionsSO<TAgent> : BaseInstructionsSO
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract CoroutineInstructions Instructions(
		TAgent agent,
		PluginData data
	);

	public override Func<IEnumerable<YieldInstruction>> GetInstructionsFor(
		GameObject agent,
		Func<bool>? keepRunningCheck = null
	) {
		PluginData data = new PluginData();
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		Action? onBegin = this.GetPluginBegin(agent, data);
		Action? onUpdate = this.GetPluginUpdate(agent, data);
		Action? onEnd = this.GetPluginEnd(agent, data);
		CoroutineInstructions instructions = this.Instructions(concreteAgent, data);

		return () => this.RunInstructions(
			instructions,
			onBegin,
			onUpdate,
			onEnd,
			keepRunningCheck
		);
	}

	private Action? GetPluginEnd(GameObject agent, PluginData data) =>
		this.plugins
			.Select(plugin => plugin.GetOnEnd(agent, data))
			.Aggregate(null as Action, (l, c) => l + c);

	private Action? GetPluginUpdate(GameObject agent, PluginData data) =>
		this.plugins
			.Select(plugin => plugin.GetOnUpdate(agent, data))
			.Aggregate(null as Action, (l, c) => l + c);

	private Action? GetPluginBegin(GameObject agent, PluginData data) =>
		this.plugins
			.Select(plugin => plugin.GetOnBegin(agent, data))
			.Aggregate(null as Action, (l, c) => l + c);

	private IEnumerable<YieldInstruction> RunInstructions(
		CoroutineInstructions instructions,
		Action? onBegin,
		Action? onUpdate,
		Action? onEnd,
		Func<bool>? run
	) {

		IEnumerable<YieldInstruction> loop = run is null
			? BaseInstructionsSO<TAgent>.Loop(instructions)
			: BaseInstructionsSO<TAgent>.Loop(instructions, run);
		onBegin?.Invoke();
		foreach (YieldInstruction hold in loop) {
			yield return hold;
			onUpdate?.Invoke();
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
		Func<bool> run
	) {
		using IEnumerator<YieldInstruction> it = instructions().GetEnumerator();
		while (it.MoveNext() && run()) {
			yield return it.Current;
		}
	}
}
