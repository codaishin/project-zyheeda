using System;
using System.Collections.Generic;

public class IntensityStack<TEffectRoutineFactory> : IStack<TEffectRoutineFactory>
	where TEffectRoutineFactory : IEffectRoutineFactory
{
	private Dictionary<Effect, Finalizable> effects = new Dictionary<Effect, Finalizable>();

	public TEffectRoutineFactory Factory { get; set; }
	public IEnumerable<Effect> Effects => this.effects.Keys;
	public event Action<Finalizable> OnPull;
	public event Action<Finalizable> OnCancel;

	public void Cancel()
	{
		this.effects.ForEach(kvp => {
			kvp.Key.Revert();
			this.OnCancel?.Invoke(kvp.Value);
		});
		this.effects.Clear();
	}

	public void Push(Effect effect)
	{
		Finalizable routine = this.Factory.Create(effect);
		routine.OnFinalize += () => this.effects.Remove(effect);
		this.effects[effect] = routine;
		this.OnPull?.Invoke(routine);
	}
}
