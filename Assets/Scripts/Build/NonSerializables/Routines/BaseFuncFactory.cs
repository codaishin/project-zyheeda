using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Routines
{
	public
	delegate IEnumerable<YieldInstruction?>? SubRoutineFn(Data data);

	public
	delegate (Action? begin, Action? update, Action? end) ModifierFn(Data data);

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

		public Reference<IModifierFactory>[] modifiers =
			new Reference<IModifierFactory>[0];

		protected virtual void ExtendData(Data data) { }

		protected abstract TAgent ConcreteAgent(GameObject agent);

		protected abstract SubRoutineFn[] SubRoutines(TAgent agent);

		private ModifierFn[] Modifiers(GameObject agent) {
			return this.modifiers
				.Values()
				.Select(factory => factory.GetModifierFnFor(agent))
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

		private (Action?, Action?, Action?) GetModifier(
			IEnumerable<ModifierFn> getModifierFns,
			Data data
		) {
			var empty = (null as Action, null as Action, null as Action);
			return getModifierFns
				.Select(getModifier => getModifier(data))
				.Aggregate(empty, Routines.Modifiers.Concat);
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
