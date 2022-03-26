using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Instructions =
	System
		.Collections
		.Generic
		.IEnumerable<UnityEngine.YieldInstruction?>;

public delegate Instructions? InstructionsFunc();
public delegate Instructions? InstructionsPluginFunc(PluginData pluginData);

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract InstructionsFunc GetInstructionsFor(
		GameObject agent,
		Func<bool>? run = null
	);
}

public class CorePluginData : PluginData
{
	public bool run;
	public float weight;
}

public abstract class BaseInstructionsSO<TAgent> : BaseInstructionsSO
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract InstructionsPluginFunc Instructions(TAgent agent);
	protected virtual void ExtendPluginData(PluginData pluginData) { }

	public override InstructionsFunc GetInstructionsFor(
		GameObject agent,
		Func<bool>? runCheck = null
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		PluginCallbacks pluginCallbacks = this.PluginCallbacks(agent);
		InstructionsPluginFunc instructions = this.Instructions(concreteAgent);

		return () => {
			CorePluginData pluginData = this.GetPluginData();
			Instructions? loopWithPluginData = instructions(pluginData);
			Func<bool> runCheckWithPluginData = runCheck is null
					? (() => pluginData.run)
					: (() => pluginData.run && runCheck());

			if (loopWithPluginData is null) {
				return null;
			}

			pluginData.run = true;
			return this.RunLoop(
				loopWithPluginData,
				() => pluginCallbacks.onBegin?.Invoke(pluginData),
				() => pluginCallbacks.onBeforeYield?.Invoke(pluginData),
				() => pluginCallbacks.onAfterYield?.Invoke(pluginData),
				() => pluginCallbacks.onEnd?.Invoke(pluginData),
				runCheckWithPluginData
			);
		};
	}

	private PluginCallbacks PluginCallbacks(GameObject agent) {
		return this.plugins
			.Select(plugin => plugin.GetCallbacks(agent))
			.Aggregate(new PluginCallbacks(), (l, c) => l + c);
	}

	private CorePluginData GetPluginData() {
		CorePluginData pluginData = new PluginData().Extent<CorePluginData>();

		this.ExtendPluginData(pluginData);
		foreach (BaseInstructionsPluginSO plugin in this.plugins) {
			plugin.ExtendPluginData(pluginData);
		}
		return pluginData;
	}

	private Func<Func<PluginData, PluginData>, Func<PluginData, PluginData>, Func<PluginData, PluginData>> Pipe() {
		return (l, c) => d => c(l(d));
	}

	private Instructions RunLoop(
		Instructions instructions,
		Action onBegin,
		Action onBeforeYield,
		Action onAfterYield,
		Action onEnd,
		Func<bool> runCheck
	) {
		onBegin();
		foreach (YieldInstruction? hold in this.RunLoop(instructions, runCheck)) {
			onBeforeYield();
			yield return hold;
			onAfterYield();
		}
		onEnd();
	}

	private Instructions RunLoop(Instructions instructions, Func<bool> runCheck) {
		using IEnumerator<YieldInstruction?> it = instructions.GetEnumerator();
		while (runCheck() && it.MoveNext()) {
			yield return it.Current;
		}
	}
}
