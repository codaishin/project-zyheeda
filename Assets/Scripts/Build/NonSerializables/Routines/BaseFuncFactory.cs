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
			private (Action? begin, Action? beginSR, Action? updateSR, Action? endSR, Action? end) modifiers;

			public Routine(
				IEnumerable<YieldInstruction?>[] yieldFns,
				(Action? begin, Action? beginSR, Action? updateSR, Action? endSR, Action? end) modifiers
			) {
				this.switched = false;
				this.switchCount = 0;
				this.yieldFns = yieldFns;
				this.modifiers = modifiers;
			}

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public IEnumerator<YieldInstruction?> GetEnumerator() {
				var (begin, _, _, _, end) = this.modifiers;
				begin?.Invoke();
				foreach (var yield in this.yieldFns.SelectMany(this.Iterate)) {
					yield return yield;
				}
				end?.Invoke();
			}

			private IEnumerable<YieldInstruction?> Iterate(
				IEnumerable<YieldInstruction?> yields
			) {
				var (_, beginSR, updateSR, endSR, _) = this.modifiers;

				beginSR?.Invoke();
				foreach (var yield in this.IterateUntilSwitched(yields)) {
					yield return yield;
					updateSR?.Invoke();
				}
				endSR?.Invoke();
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

		private (Action?, Action?, Action?, Action?, Action?) GetModifier(
			IEnumerable<(ModifierFn, ModifierHook)> getModifierFnData,
			RoutineData data
		) {
			(Action?, ModifierHook) GetModifierFnForData(
				(ModifierFn, ModifierHook) getModifierFnData
			) {
				var (getModifierFn, hook) = getModifierFnData;
				return (getModifierFn(data), hook);
			}

			(Action?, Action?, Action?, Action?, Action?) Concat(
				(Action?, Action?, Action?, Action?, Action?) aggregate,
				(Action?, ModifierHook) current
			) {
				var (act, h) = current;
				var (begin, beginSR, updateSR, endSR, end) = aggregate;
				return (
					begin + (h.HasFlag(ModifierHook.OnBegin) ? act : null),
					beginSR + (h.HasFlag(ModifierHook.OnBeginSubRoutine) ? act : null),
					updateSR + (h.HasFlag(ModifierHook.OnUpdateSubRoutine) ? act : null),
					endSR + (h.HasFlag(ModifierHook.OnEndSubroutine) ? act : null),
					end + (h.HasFlag(ModifierHook.OnEnd) ? act : null)
				);
			}

			var empty = (
				begin: null as Action,
				beginSR: null as Action,
				updateSR: null as Action,
				endSR: null as Action,
				end: null as Action
			);

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
