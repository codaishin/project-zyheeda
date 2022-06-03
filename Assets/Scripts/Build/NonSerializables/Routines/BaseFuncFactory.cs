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
		private class ModifierCallbacks :
			Tuple<Action?, Action?, Action?, Action?, Action?>
		{
			public
			ModifierCallbacks() : base(null, null, null, null, null) { }

			public
			ModifierCallbacks(
				ModifierCallbacks source,
				Action? action,
				ModifierHook hook
			) : base(
					source.Item1 + (
						hook.HasFlag(ModifierHook.OnBegin) ? action : null
					),
					source.Item2 + (
						hook.HasFlag(ModifierHook.OnBeginSubRoutine) ? action : null
					),
					source.Item3 + (
						hook.HasFlag(ModifierHook.OnUpdateSubRoutine) ? action : null
					),
					source.Item4 + (
						hook.HasFlag(ModifierHook.OnEndSubroutine) ? action : null
					),
					source.Item5 + (
						hook.HasFlag(ModifierHook.OnEnd) ? action : null
					)
			) { }
		}

		private class Routine : IRoutine
		{
			private bool switched;
			private int switchCount;
			private IEnumerable<YieldInstruction?>[] yieldFns;
			private ModifierCallbacks modifiers;

			public
			Routine(IEnumerable<YieldInstruction?>[] yieldFns, ModifierCallbacks modifiers) {
				this.switched = false;
				this.switchCount = 0;
				this.yieldFns = yieldFns;
				this.modifiers = modifiers;
			}

			IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

			public
			IEnumerator<YieldInstruction?> GetEnumerator() {
				var (begin, _, _, _, end) = this.modifiers;
				begin?.Invoke();
				foreach (var yield in this.yieldFns.SelectMany(this.Iterate)) {
					yield return yield;
				}
				end?.Invoke();
			}

			private
			IEnumerable<YieldInstruction?> Iterate(
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

			private
			IEnumerable<YieldInstruction?> IterateUntilSwitched(
				IEnumerable<YieldInstruction?> yields
			) {
				using var enumerator = yields.GetEnumerator();

				this.switched = false;
				while (this.switched == false && enumerator.MoveNext()) {
					yield return enumerator.Current;
				}
			}

			public
			bool NextSubRoutine() {
				this.switched = true;
				return ++this.switchCount < this.yieldFns.Length;
			}
		}

		public ModifierData[] modifiers = new ModifierData[0];

		protected
		virtual
		void ExtendData(Data data) { }

		protected
		abstract
		TAgent ConcreteAgent(GameObject agent);

		protected
		abstract
		SubRoutineFn[] SubRoutines(TAgent agent);

		private
		(ModifierFn, ModifierHook)[] Modifiers(GameObject agent) {
			return this.modifiers
				.Select(d => (d.factory.Value!.GetModifierFnFor(agent), d.hook))
				.ToArray();
		}

		private
		Data GetData() {
			var data = new Data();
			this.ExtendData(data);
			return data;
		}

		private
		IEnumerable<IEnumerable<YieldInstruction?>> GetSubRoutines(
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

		private
		ModifierCallbacks GetModifier(
			IEnumerable<(ModifierFn, ModifierHook)> getModifierFnData,
			Data data
		) {
			(Action?, ModifierHook) GetModifierFnForData(
				(ModifierFn, ModifierHook) getModifierFnData
			) {
				var (getModifierFn, hook) = getModifierFnData;
				return (getModifierFn(data), hook);
			}

			ModifierCallbacks Concat(
				ModifierCallbacks aggregate,
				(Action?, ModifierHook) current
			) {
				var (action, hook) = current;
				return new ModifierCallbacks(aggregate, action, hook);
			}

			return getModifierFnData
				.Select(GetModifierFnForData)
				.Aggregate(new ModifierCallbacks(), Concat);
		}

		public
		Func<IRoutine?> GetRoutineFnFor(GameObject agent) {
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
