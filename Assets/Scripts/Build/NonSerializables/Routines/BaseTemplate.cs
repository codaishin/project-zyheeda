using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Routines
{
	public delegate IEnumerable<YieldInstruction?>? SubRoutineFn(Data data);
	public delegate Action? PluginFn(Data data);

	public abstract class BaseTemplate<TAgent> : ITemplate
	{
		private class Routine : IRoutine
		{
			private bool switched;
			private int switchCount;
			private IEnumerable<YieldInstruction?>[] yieldFns;
			private Dictionary<PluginFlags, Action> modifiers;

			public Routine(
				IEnumerable<YieldInstruction?>[] yieldFns,
				Dictionary<PluginFlags, Action> modifiers
			) {
				this.switched = false;
				this.switchCount = 0;
				this.yieldFns = yieldFns;
				this.modifiers = modifiers;
			}

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public IEnumerator<YieldInstruction?> GetEnumerator() {
				var (begin, end) = (
					this.modifiers.GetValueOrDefault(PluginFlags.OnBegin),
					this.modifiers.GetValueOrDefault(PluginFlags.OnEnd)
				);
				begin?.Invoke();
				foreach (var yield in this.yieldFns.SelectMany(this.Iterate)) {
					yield return yield;
				}
				end?.Invoke();
			}

			private IEnumerable<YieldInstruction?> Iterate(
				IEnumerable<YieldInstruction?> yields
			) {
				var (begin, update, end) = (
					this.modifiers.GetValueOrDefault(PluginFlags.OnBeginSubRoutine),
					this.modifiers.GetValueOrDefault(PluginFlags.OnUpdateSubRoutine),
					this.modifiers.GetValueOrDefault(PluginFlags.OnEndSubroutine)
				);
				begin?.Invoke();
				foreach (var yield in this.IterateUntilSwitched(yields)) {
					yield return yield;
					update?.Invoke();
				}
				end?.Invoke();
			}

			private IEnumerable<YieldInstruction?> IterateUntilSwitched(
				IEnumerable<YieldInstruction?> yields
			) {
				using var enumerator = yields.GetEnumerator();

				this.switched = false;
				while (this.switched == false && enumerator.MoveNext()) {
					yield return enumerator.Current;
				}
			}

			public bool NextSubRoutine() {
				this.switched = true;
				return ++this.switchCount < this.yieldFns.Length;
			}
		}

		public PluginData[] plugins = new PluginData[0];

		protected virtual void ExtendData(Data data) { }

		protected abstract TAgent ConcreteAgent(GameObject agent);

		protected abstract SubRoutineFn[] SubRoutines(TAgent agent);

		private (PluginFn, PluginFlags)[] Modifiers(GameObject agent) {
			return this.plugins
				.Select(d => (d.plugin.Value!.GetPluginFnFor(agent), d.flag))
				.ToArray();
		}

		private Data GetData() {
			var data = new Data();
			this.ExtendData(data);
			return data;
		}

		private IEnumerable<IEnumerable<YieldInstruction?>> GetSubRoutines(
			IEnumerable<SubRoutineFn> yieldFns,
			Data data
		) {
			var yields = null as IEnumerable<YieldInstruction?>;

			foreach (var yieldsFn in yieldFns) {
				if ((yields = yieldsFn(data)) is not null) {
					yield return yields;
				}
			}
		}

		private Dictionary<PluginFlags, Action> GetModifiers(
			IEnumerable<(PluginFn, PluginFlags)> getModifierFnData,
			Data data
		) {
			(Action?, PluginFlags) GetModifierFnForData(
				(PluginFn, PluginFlags) getModifierFnData
			) {
				var (getModifierFn, hook) = getModifierFnData;
				return (getModifierFn(data), hook);
			}

			Dictionary<PluginFlags, Action> ConcatFlagActions(
				Dictionary<PluginFlags, Action> modifiers,
				(Action?, PluginFlags) current
			) {
				var (action, hook) = current;
				if (action is null) {
					return modifiers;
				}
				foreach (var flag in hook.GetFlags()) {
					var concat = modifiers.GetValueOrDefault(flag) + action;
					modifiers[flag] = concat;
				}
				return modifiers;
			}

			return getModifierFnData
				.Select(GetModifierFnForData)
				.Aggregate(new Dictionary<PluginFlags, Action>(), ConcatFlagActions);
		}

		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) {
			var cAgent = this.ConcreteAgent(agent);
			var modifierFns = this.Modifiers(agent);
			var subRoutineFns = this.SubRoutines(cAgent);

			return () => {
				var data = this.GetData();
				var modifiers = this.GetModifiers(modifierFns, data);
				var subRoutines = this.GetSubRoutines(subRoutineFns, data).ToArray();

				if (subRoutines.Length == 0) {
					return null;
				}
				return new Routine(subRoutines, modifiers);
			};
		}
	}
}
