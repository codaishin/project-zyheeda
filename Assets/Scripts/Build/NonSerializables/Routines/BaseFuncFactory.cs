using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Routines
{
	public delegate IEnumerable<YieldInstruction?>? SubRoutineFn(RoutineData data);
	public delegate Action? ModifierFn(RoutineData data);

	public abstract class BaseFuncFactory<TAgent> : IFuncFactory
	{
		private class Routine : IRoutine
		{
			private bool switched;
			private int switchCount;
			private IEnumerable<YieldInstruction?>[] yieldFns;
			private (Action? begin, Action? update, Action? end) modifiers;

			public Routine(
				IEnumerable<YieldInstruction?>[] yieldFns,
				(Action? begin, Action? update, Action? end) modifiers
			) {
				this.switched = false;
				this.switchCount = 0;
				this.yieldFns = yieldFns;
				this.modifiers = modifiers;
			}

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public IEnumerator<YieldInstruction?> GetEnumerator() {
				foreach (var yield in this.yieldFns.SelectMany(this.Iterate)) {
					yield return yield;
				}
			}

			private IEnumerable<YieldInstruction?> Iterate(
				IEnumerable<YieldInstruction?> yields
			) {
				var (begin, update, end) = this.modifiers;

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

		protected virtual void ExtendData(RoutineData data) { }

		protected abstract TAgent ConcreteAgent(GameObject agent);

		protected abstract SubRoutineFn[] SubRoutines(TAgent agent);

		private (ModifierFn, ModifierHook)[] Modifiers(GameObject agent) {
			return this.modifiers
				.Select(d => (d.factory.Value!.GetModifierFnFor(agent), d.hook))
				.ToArray();
		}

		private RoutineData GetData() {
			var data = new RoutineData();
			this.ExtendData(data);
			return data;
		}

		private IEnumerable<IEnumerable<YieldInstruction?>> GetSubRoutines(
			IEnumerable<SubRoutineFn> yieldFns,
			RoutineData data
		) {
			var yields = null as IEnumerable<YieldInstruction?>;

			foreach (var yieldsFn in yieldFns) {
				if ((yields = yieldsFn(data)) is not null) {
					yield return yields;
				}
			}
		}

		private (Action?, Action?, Action?) GetModifier(
			IEnumerable<(ModifierFn, ModifierHook)> getModifierFnData,
			RoutineData data
		) {
			var empty = (null as Action, null as Action, null as Action);

			(Action?, ModifierHook) GetModifierFnForData(
				(ModifierFn, ModifierHook) getModifierFnData
			) {
				var (getModifierFn, hook) = getModifierFnData;
				return (getModifierFn(data), hook);
			}

			(Action?, Action?, Action?) Concat(
				(Action?, Action?, Action?) aggregate,
				(Action?, ModifierHook) current
			) {
				var (action, hook) = current;
				var (begin, update, end) = aggregate;
				return (
					begin + (hook.HasFlag(ModifierHook.OnBegin) ? action : null),
					update + (hook.HasFlag(ModifierHook.OnUpdate) ? action : null),
					end + (hook.HasFlag(ModifierHook.OnEnd) ? action : null)
				);
			}

			return getModifierFnData
				.Select(GetModifierFnForData)
				.Aggregate(empty, Concat);
		}

		public Func<IRoutine?> GetRoutineFnFor(GameObject agent) {
			var cAgent = this.ConcreteAgent(agent);
			var modifierFns = this.Modifiers(agent);
			var subRoutineFns = this.SubRoutines(cAgent);

			return () => {
				var data = this.GetData();
				var modifiers = this.GetModifier(modifierFns, data);
				var subRoutines = this.GetSubRoutines(subRoutineFns, data).ToArray();

				if (subRoutines.Length == 0) {
					return null;
				}
				return new Routine(subRoutines, modifiers);
			};
		}
	}
}
