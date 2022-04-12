using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Instructions =
	System
		.Collections
		.Generic
		.IEnumerable<UnityEngine.YieldInstruction?>;

public struct InstructionData
{
	public readonly Instructions instructions;
	public readonly Action release;

	public InstructionData(Instructions instructions, Action release) {
		this.instructions = instructions;
		this.release = release;
	}

	public void Deconstruct(out Instructions instructions, out Action release) {
		instructions = this.instructions;
		release = this.release;
	}
}

public delegate InstructionData? ExternalInstructionsFn();
public delegate Instructions? InternalInstructionFn(PluginData data);
public delegate PluginHooks PluginHooksFn(PluginData data);

public class CorePluginData : PluginData
{
	public bool run;
	public float weight;
}

public abstract class BaseInstructionsTemplate<TAgent> : IInstructionsTemplate
{
	public Reference<IPlugin>[] plugins = new Reference<IPlugin>[0];

	protected abstract TAgent ConcreteAgent(GameObject agent);
	protected abstract InternalInstructionFn InternalInstructionsFn(TAgent agent);
	protected virtual void ExtendPluginData(PluginData pluginData) { }

	public ExternalInstructionsFn GetInstructionsFor(GameObject agent) {
		var concreteAgent = this.ConcreteAgent(agent);
		var instructionFn = this.InternalInstructionsFn(concreteAgent);
		var pluginHooksFns = this.PluginHooksFns(agent);

		return () => {
			var pluginData = this.CorePluginData();
			var pluginHooks = this.FinalizePluginHooks(pluginHooksFns, pluginData);
			var instructions = instructionFn(pluginData);

			if (instructions is null) {
				return null;
			}

			pluginData.run = true;
			return new InstructionData(
				this.Run(instructions, pluginHooks, pluginData),
				() => pluginData.run = false
			);
		};
	}

	private IEnumerable<PluginHooksFn> PluginHooksFns(GameObject agent) {
		return this.plugins
			.Values()
			.Select(plugin => plugin.PluginHooksFor(agent));
	}

	private PluginHooks FinalizePluginHooks(
		IEnumerable<PluginHooksFn> getPluginHooksFns,
		CorePluginData data
	) {
		return getPluginHooksFns
			.Select(getPluginHooksFn => getPluginHooksFn(data))
			.Aggregate(new PluginHooks(), PluginHooks.Concat);
	}

	private CorePluginData CorePluginData() {
		var corePluginData = new CorePluginData();
		this.ExtendPluginData(corePluginData);
		return corePluginData;
	}

	private Instructions Run(
		Instructions instructions,
		PluginHooks pluginHooks,
		CorePluginData pluginData
	) {
		var run = BaseInstructionsTemplate<TAgent>.Run(instructions, pluginData);

		pluginHooks.onBegin?.Invoke();
		foreach (YieldInstruction? hold in run) {
			yield return hold;
			pluginHooks.onUpdate?.Invoke();
		}
		pluginHooks.onEnd?.Invoke();
	}

	private static Instructions Run(
		Instructions instructions,
		CorePluginData pluginData
	) {
		using var enumerator = instructions.GetEnumerator();
		while (pluginData.run && enumerator.MoveNext()) {
			yield return enumerator.Current;
		}
	}
}
