using System;
using System.Collections;
using System.Collections.Generic;

public class DurationStack<TEffectRoutineFactory> : IStack<TEffectRoutineFactory>
	where TEffectRoutineFactory : IEffectRoutineFactory
{
	private Queue<Effect> effects = new Queue<Effect>();
	private Finalizable wrapper;

	public TEffectRoutineFactory Factory { get; set; }
	public IEnumerable<Effect> Effects => this.effects;
	public event Action<Finalizable> OnPull;
	public event Action<Finalizable> OnCancel;

	public void Cancel()
	{
		this.OnCancel?.Invoke(this.wrapper);
		this.wrapper = null;
		if (this.effects.Count > 0) {
			this.effects.Peek().Revert();
		}
		this.effects.Clear();
	}

	private IEnumerator GetNext()
	{
		while (this.effects.Count > 0) {
			Finalizable routine = this.Factory.Create(this.effects.Peek());
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
			this.OnPull?.Invoke(this.wrapper);
		}
	}
}
