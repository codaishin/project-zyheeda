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
public delegate Instructions? InstructionsPluginFunc(CorePluginData pluginData);

public abstract class BaseInstructionsSO : ScriptableObject
{
	public abstract InstructionsFunc GetInstructionsFor(
		GameObject agent,
		Func<bool>? run = null
	);
}

public abstract class BaseInstructionsSO<TAgent> : BaseInstructionsSO
{
	public BaseInstructionsPluginSO[] plugins = new BaseInstructionsPluginSO[0];

	protected abstract TAgent GetConcreteAgent(GameObject agent);
	protected abstract InstructionsPluginFunc Instructions(TAgent agent);

	public override InstructionsFunc GetInstructionsFor(
		GameObject agent,
		Func<bool>? runCheck = null
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		PluginCallbacks pluginCallbacks = this.PluginCallbacks(agent);
		InstructionsPluginFunc instructions = this.Instructions(concreteAgent);

		return () => {
			CorePluginData pluginData = new CorePluginData { run = true };
			Instructions? loop = instructions(pluginData);

			if (loop == null) {
				return null;
			}
			return this.RunLoop(
				loop,
				() => pluginCallbacks.onBegin?.Invoke(pluginData),
				() => pluginCallbacks.onBeforeYield?.Invoke(pluginData),
				() => pluginCallbacks.onAfterYield?.Invoke(pluginData),
				() => pluginCallbacks.onEnd?.Invoke(pluginData),
				runCheck + (() => pluginData.run)
			);
		};
	}

	private PluginCallbacks PluginCallbacks(GameObject agent) {
		return this.plugins
			.Select(plugin => plugin.GetCallbacks(agent))
			.Aggregate(new PluginCallbacks(), (l, c) => l + c);
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
