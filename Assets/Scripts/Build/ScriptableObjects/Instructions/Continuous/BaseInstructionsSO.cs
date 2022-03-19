using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract Func<IEnumerable<YieldInstruction>> GetInstructionsFor(
		GameObject agent,
		Func<bool>? run = null
	);

	protected static IEnumerable<YieldInstruction> RunInstructions(
		RawInstructions instructions,
		Action<PluginData>? onBegin,
		Action<PluginData>? onBeforeYield,
		Action<PluginData>? onAfterYield,
		Action<PluginData>? onEnd,
		Func<bool>? run
	) {
		PluginData pluginData = new PluginData { run = true };
		Func<bool> runCheck = run is null
			? () => pluginData.run
			: () => pluginData.run && run();
		IEnumerable<YieldInstruction> loop = BaseInstructionsSO.Loop(
			instructions(pluginData),
			runCheck
		);
		onBegin?.Invoke(pluginData);
		foreach (YieldInstruction hold in loop) {
			onBeforeYield?.Invoke(pluginData);
			yield return hold;
			onAfterYield?.Invoke(pluginData);
		}
		onEnd?.Invoke(pluginData);
	}

	protected static IEnumerable<YieldInstruction> Loop(
		IEnumerable<YieldInstruction> instructions,
		Func<bool> run
	) {
		using IEnumerator<YieldInstruction> it = instructions.GetEnumerator();
		while (run() && it.MoveNext()) {
			yield return it.Current;
		}
	}
}

public delegate IEnumerable<YieldInstruction> CoroutineInstructions();
public delegate IEnumerable<YieldInstruction> RawInstructions(PluginData data);

public abstract class BaseInstructionsSO<TAgent> : BaseInstructionsSO
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract RawInstructions Instructions(TAgent agent);

	public override Func<IEnumerable<YieldInstruction>> GetInstructionsFor(
		GameObject agent,
		Func<bool>? run = null
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		PluginCallbacks callbacks = this.PluginCallbacks(agent);
		RawInstructions instructions = this.Instructions(concreteAgent);

		return () => BaseInstructionsSO.RunInstructions(
			instructions,
			callbacks.onBegin,
			callbacks.onBeforeYield,
			callbacks.onAfterYield,
			callbacks.onEnd,
			run
		);
	}

	private PluginCallbacks PluginCallbacks(GameObject agent) {
		return this.plugins
			.Select(plugin => plugin.GetCallbacks(agent))
			.Aggregate(new PluginCallbacks(), (l, c) => l + c);
	}
}
