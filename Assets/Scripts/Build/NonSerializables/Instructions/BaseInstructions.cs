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
public delegate Instructions? PartialInstructionFunc(PluginData data);
public delegate PluginCallbacks PartialPluginCallbacks(PluginData data);

public class CorePluginData : PluginData
{
	public bool run;
	public float weight;
}

public abstract class BaseInstructions<TAgent> : IInstructions
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract PartialInstructionFunc PartialInstructions(TAgent agent);
	protected virtual void ExtendPluginData(PluginData pluginData) { }

	public InstructionsFunc GetInstructionsFor(
		GameObject agent,
		Func<bool>? runCheck = null
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		PartialInstructionFunc instructions = this.PartialInstructions(concreteAgent);
		IEnumerable<PartialPluginCallbacks> partialPluginCalls =
			this.PartialPluginCalls(agent);

		return () => {
			CorePluginData data = this.GetPluginData();
			Instructions? loop = instructions(data);
			PluginCallbacks pluginCalls =
				this.FinalizePluginCalls(partialPluginCalls, data);
			Func<bool> runCheckWithPluginData =
				runCheck is null
					? (() => data.run)
					: (() => data.run && runCheck());

			if (loop is null) {
				return null;
			}

			data.run = true;
			return this.RunLoop(loop, pluginCalls, runCheckWithPluginData);
		};
	}

	private IEnumerable<PartialPluginCallbacks> PartialPluginCalls(
		GameObject agent
	) {
		return this.plugins.Select(plugin => plugin.GetCallbacks(agent));
	}

	private PluginCallbacks FinalizePluginCalls(
		IEnumerable<PartialPluginCallbacks> partialPluginCallbacks,
		PluginData data
	) {
		PluginCallbacks empty = new PluginCallbacks();
		return partialPluginCallbacks
			.Select(partial => partial(data))
			.Aggregate(empty, (last, current) => last + current);
	}

	private CorePluginData GetPluginData() {
		PluginData pluginData = new PluginData();
		this.ExtendPluginData(pluginData);
		return pluginData.Extent<CorePluginData>();
	}

	private Instructions RunLoop(
		Instructions loop,
		PluginCallbacks pluginCalls,
		Func<bool> runCheck
	) {
		pluginCalls.onBegin?.Invoke();
		foreach (YieldInstruction? hold in this.RunLoop(loop, runCheck)) {
			pluginCalls.onBeforeYield?.Invoke();
			yield return hold;
			pluginCalls.onAfterYield?.Invoke();
		}
		pluginCalls.onEnd?.Invoke();
	}

	private Instructions RunLoop(Instructions loop, Func<bool> runCheck) {
		using IEnumerator<YieldInstruction?> it = loop.GetEnumerator();
		while (runCheck() && it.MoveNext()) {
			yield return it.Current;
		}
	}
}
