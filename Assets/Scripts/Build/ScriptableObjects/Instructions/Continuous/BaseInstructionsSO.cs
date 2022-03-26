using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Instructions =
	System.Collections.Generic.IEnumerable<UnityEngine.YieldInstruction?>;

public delegate Instructions? InstructionsFunc();
public delegate Instructions? InstructionsPluginFunc(PluginData pluginData);

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
		Func<bool>? run = null
	) {
		TAgent concreteAgent = this.GetConcreteAgent(agent);
		PluginCallbacks callbacks = this.PluginCallbacks(agent);
		InstructionsPluginFunc instructionsFunc = this.Instructions(concreteAgent);

		return () => {
			PluginData pluginData = new PluginData { run = true };
			Instructions? instructions = instructionsFunc(pluginData);
			Func<bool> runCheck = run is null
				? (() => pluginData.run)
				: (() => pluginData.run && run());

			if (instructions == null) {
				return null;
			}
			return BaseInstructionsSO<TAgent>.Loop(
				instructions,
				() => callbacks.onBegin?.Invoke(pluginData),
				() => callbacks.onBeforeYield?.Invoke(pluginData),
				() => callbacks.onAfterYield?.Invoke(pluginData),
				() => callbacks.onEnd?.Invoke(pluginData),
				runCheck
			);
		};
	}

	private PluginCallbacks PluginCallbacks(GameObject agent) {
		return this.plugins
			.Select(plugin => plugin.GetCallbacks(agent))
			.Aggregate(new PluginCallbacks(), (l, c) => l + c);
	}

	private static Instructions Loop(
		Instructions instructions,
		Action onBegin,
		Action onBeforeYield,
		Action onAfterYield,
		Action onEnd,
		Func<bool> runCheck
	) {
		IEnumerable<YieldInstruction?> loop = BaseInstructionsSO<TAgent>.Loop(
			instructions,
			runCheck
		);
		onBegin();
		foreach (YieldInstruction? hold in loop) {
			onBeforeYield();
			yield return hold;
			onAfterYield();
		}
		onEnd();
	}

	private static Instructions Loop(
		Instructions instructions,
		Func<bool> runCheck
	) {
		using IEnumerator<YieldInstruction?> it = instructions.GetEnumerator();
		while (runCheck() && it.MoveNext()) {
			yield return it.Current;
		}
	}
}
