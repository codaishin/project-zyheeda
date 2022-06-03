using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Routines
{
	public delegate IEnumerable<YieldInstruction?>? SubRoutineFn(Data data);
	public delegate Action? ModifierFn(Data data);

	public abstract class BaseFuncFactory<TAgent> : IFuncFactory
	{
		private class Routine : IRoutine
		{
			private bool switched;
			private int switchCount;
			private IEnumerable<YieldInstruction?>[] yieldFns;
			private Dictionary<ModifierFlags, Action> modifiers;

			public Routine(
				IEnumerable<YieldInstruction?>[] yieldFns,
				Dictionary<ModifierFlags, Action> modifiers
			) {
				this.switched = false;
				this.switchCount = 0;
				this.yieldFns = yieldFns;
				this.modifiers = modifiers;
			}

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public IEnumerator<YieldInstruction?> GetEnumerator() {
				var (begin, end) = (
					this.modifiers.GetValueOrDefault(ModifierFlags.OnBegin),
					this.modifiers.GetValueOrDefault(ModifierFlags.OnEnd)
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
					this.modifiers.GetValueOrDefault(ModifierFlags.OnBeginSubRoutine),
					this.modifiers.GetValueOrDefault(ModifierFlags.OnUpdateSubRoutine),
					this.modifiers.GetValueOrDefault(ModifierFlags.OnEndSubroutine)
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

		public ModifierData[] modifiers = new ModifierData[0];

		protected virtual void ExtendData(Data data) { }

		protected abstract TAgent ConcreteAgent(GameObject agent);

		protected abstract SubRoutineFn[] SubRoutines(TAgent agent);

		private (ModifierFn, ModifierFlags)[] Modifiers(GameObject agent) {
			return this.modifiers
				.Select(d => (d.factory.Value!.GetModifierFnFor(agent), d.hook))
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

		private Dictionary<ModifierFlags, Action> GetModifiers(
			IEnumerable<(ModifierFn, ModifierFlags)> getModifierFnData,
			Data data
		) {
			(Action?, ModifierFlags) GetModifierFnForData(
				(ModifierFn, ModifierFlags) getModifierFnData
			) {
				var (getModifierFn, hook) = getModifierFnData;
				return (getModifierFn(data), hook);
			}

			Dictionary<ModifierFlags, Action> ConcatFlagActions(
				Dictionary<ModifierFlags, Action> modifiers,
				(Action?, ModifierFlags) current
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
				.Aggregate(new Dictionary<ModifierFlags, Action>(), ConcatFlagActions);
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
