using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract Func<IEnumerable<YieldInstruction>> GetInstructionsFor(
		GameObject agent,
		PluginData? data = null
	);

	protected static IEnumerable<YieldInstruction> RunInstructions(
		CoroutineInstructions instructions,
		Action? onBegin,
		Action? onUpdate,
		Action? onEnd,
		PluginData data
	) {

		IEnumerable<YieldInstruction> loop =
			BaseInstructionsSO.Loop(instructions, data);
		onBegin?.Invoke();
		foreach (YieldInstruction hold in loop) {
			yield return hold;
			onUpdate?.Invoke();
		}
		onEnd?.Invoke();
	}

	protected static IEnumerable<YieldInstruction> Loop(
		CoroutineInstructions instructions,
		PluginData data
	) {
		using IEnumerator<YieldInstruction> it = instructions().GetEnumerator();
		while (data.run && it.MoveNext()) {
			yield return it.Current;
		}
	}
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
		PluginData? data = null
	) {
		data = data ?? new PluginData { run = true };
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		PluginCallbacks callbacks = this.PluginCallbacks(agent, data);
		CoroutineInstructions instructions = this.Instructions(concreteAgent, data);

		return () => BaseInstructionsSO.RunInstructions(
			instructions,
			callbacks.onBegin,
			callbacks.onUpdate,
			callbacks.onEnd,
			data
		);
	}

	private PluginCallbacks PluginCallbacks(GameObject agent, PluginData data) =>
		this.plugins
			.Select(plugin => plugin.GetCallbacks(agent, data))
			.Aggregate(new PluginCallbacks(), (l, c) => new PluginCallbacks {
				onBegin = l.onBegin + c.onBegin,
				onUpdate = l.onUpdate + c.onUpdate,
				onEnd = l.onEnd + c.onEnd,
			});
}
