using System;
using System.Collections.Generic;

public static class IntensityStackFactory
{
	private class Stack : IStack
	{
		private Dictionary<Effect, Finalizable> effects = new Dictionary<Effect, Finalizable>();
		private Func<Effect, Finalizable> effectToRoutine;
		private Action<Finalizable> onPull;
		private Action<Finalizable> onCancel;

		public IEnumerable<Effect> Effects => this.effects.Keys;

		public Stack(Func<Effect, Finalizable> effectToRoutine, Action<Finalizable> onPull, Action<Finalizable> onCancel)
		{
			this.effectToRoutine = effectToRoutine;
			this.onPull = onPull;
			this.onCancel = onCancel;
		}

		public void Cancel()
		{
			this.effects.ForEach(kvp => {
				kvp.Key.Revert();
				this.onCancel?.Invoke(kvp.Value);
			});
			this.effects.Clear();
		}

		public void Push(Effect effect)
		{
			Finalizable routine = this.effectToRoutine(effect);
			routine.OnFinalize += () => effects.Remove(effect);
			effects[effect] = routine;
			onPull?.Invoke(routine);
		}
	}

	public static IStack Create(
		Func<Effect, Finalizable> effectToRoutine,
		Action<Finalizable> onPull = default,
		Action<Finalizable> onCancel = default
	) => new Stack(effectToRoutine, onPull, onCancel);
}
