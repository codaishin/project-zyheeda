using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StackFactories/Duration")]
public class DurationStackFactorySO : BaseStackFactorySO
{
	private class Stack : IStack
	{
		private Queue<Effect> effects = new Queue<Effect>();
		private Finalizable wrapper;
		private Func<Effect, Finalizable> effectToRoutine;
		private Action<Finalizable> onPull;
		private Action<Finalizable> onCancel;

		public IEnumerable<Effect> Effects => this.effects;

		public Stack(Func<Effect, Finalizable> effectToRoutine, Action<Finalizable> onPull, Action<Finalizable> onCancel)
		{
			this.effectToRoutine = effectToRoutine;
			this.onPull = onPull;
			this.onCancel = onCancel;
		}

		public void Cancel()
		{
			if (this.wrapper != null && this.onCancel != null) {
				this.onCancel(this.wrapper);
			}
			if (this.effects.Count > 0) {
				this.effects.Peek().Revert();
			}
			this.wrapper = null;
			this.effects.Clear();
		}

		private IEnumerator GetNext()
		{
			while (this.effects.Count > 0) {
				Finalizable routine = this.effectToRoutine(this.effects.Peek());
				while (routine.MoveNext()) {
					yield return routine.Current;
				}
				this.effects.Dequeue();
			}
		}

		public void Push(Effect effect)
		{
			this.effects.Enqueue(effect);
			if (this.wrapper == null) {
				this.wrapper = new Finalizable{ wrapped = this.GetNext() };
				this.wrapper.OnFinalize += () => this.wrapper = null;
				this.onPull?.Invoke(this.wrapper);
			}
		}
	}

	public override IStack Create(
		Func<Effect, Finalizable> effectToRoutine,
		Action<Finalizable> onPull = default,
		Action<Finalizable> onCancel = default
	) => new Stack(effectToRoutine, onPull, onCancel);
}
