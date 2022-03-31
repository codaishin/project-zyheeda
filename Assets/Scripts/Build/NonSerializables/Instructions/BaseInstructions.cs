using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Instructions =
	System
		.Collections
		.Generic
		.IEnumerable<UnityEngine.YieldInstruction?>;

public delegate Instructions? InstructionsFunc(Func<bool>? run = null);
public delegate Instructions? PartialInstructionFunc(PluginData data);
public delegate PluginCallbacks PartialPluginCallbacks(PluginData data);

public class CorePluginData : PluginData
{
	public bool run;
	public float weight;
}

public abstract class BaseInstructions<TAgent> : IInstructionsTemplate
{
	public Reference<IPlugin>[] plugins = new Reference<IPlugin>[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract PartialInstructionFunc PartialInstructions(TAgent agent);
	protected virtual void ExtendPluginData(PluginData pluginData) { }

	public InstructionsFunc GetInstructionsFor(GameObject agent) {
		TAgent concreteAgent =
			this.GetConcreteAgent(agent);
		PartialInstructionFunc instructions =
			this.PartialInstructions(concreteAgent);
		IEnumerable<PartialPluginCallbacks> partialPluginCallbacks =
			this.GetPartialPluginCallbacksFor(agent);

		return runCheck => {
			CorePluginData data =
				this.GetCorePluginData();
			Instructions? loop =
				instructions(data);
			PluginCallbacks pluginCalls =
				this.FinalizePluginCalls(partialPluginCallbacks, data);
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

	private IEnumerable<PartialPluginCallbacks> GetPartialPluginCallbacksFor(
		GameObject agent
	) {
		return this.plugins
			.Values()
			.Select(plugin => plugin.GetCallbacks(agent));
	}

	private PluginCallbacks FinalizePluginCalls(
		IEnumerable<PartialPluginCallbacks> partialPluginCallbacks,
		CorePluginData data
	) {
		PluginCallbacks emptyPCB = new PluginCallbacks();
		return partialPluginCallbacks
			.Select(partialPCB => partialPCB(data))
			.Aggregate(emptyPCB, (lastPCB, currentPCB) => lastPCB + currentPCB);
	}

	private CorePluginData GetCorePluginData() {
		CorePluginData corePluginData = new CorePluginData();
		this.ExtendPluginData(corePluginData);
		return corePluginData;
	}

	private Instructions RunLoop(
		Instructions loop,
		PluginCallbacks pluginCalls,
		Func<bool> runCheck
	) {
		IEnumerable<YieldInstruction?> runLoop =
			BaseInstructions<TAgent>.RunLoop(loop, runCheck);

		pluginCalls.onBegin?.Invoke();
		foreach (YieldInstruction? hold in runLoop) {
			pluginCalls.onBeforeYield?.Invoke();
			yield return hold;
			pluginCalls.onAfterYield?.Invoke();
		}
		pluginCalls.onEnd?.Invoke();
	}

	private static Instructions RunLoop(Instructions loop, Func<bool> runCheck) {
		using IEnumerator<YieldInstruction?> enumerator = loop.GetEnumerator();
		while (runCheck() && enumerator.MoveNext()) {
			yield return enumerator.Current;
		}
	}
}
