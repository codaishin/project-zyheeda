using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseConditionManagerMB<TCreator, TStacking> : MonoBehaviour, IConditionManager
	where TCreator: IEffectRoutineCreator, new()
	where TStacking : IEffectRoutineStacking, new()
{
	private class EffectStack
	{
		public List<(Effect value, Action revert)> effects = new List<(Effect, Action)>();
		public List<Finalizable> routines = new List<Finalizable>();
	}

	private Dictionary<EffectTag, EffectStack> stacks = new Dictionary<EffectTag, EffectStack>();
	public TCreator effectRoutineCreator = new TCreator();
	public TStacking effectRoutineStacking = new TStacking();


	public IEnumerable<Effect> GetEffects(EffectTag tag) => this.stacks[tag].effects.Select(e => e.value);

	private EffectStack GetOrCreateStack(EffectTag tag)
	{
		if (!this.stacks.TryGetValue(tag, out EffectStack stack)) {
			stack = new EffectStack();
			this.stacks[tag] = stack;
		}
		return stack;
	}

	private void StoreEffect(Effect effect, Action revert, Finalizable effectRoutine, EffectStack stack)
	{
		(Effect, Action) cache = (effect, revert);
		effectRoutine.OnFinalize += () => stack.effects.Remove(cache);
		stack.effects.Add(cache);
	}


	private void StackEffectRoutine(Finalizable effectRoutine, EffectStack stack)
	{
		this.effectRoutineStacking.Add(effectRoutine, stack.routines, added => {
			added.OnFinalize += () => stack.routines.Remove(added);
			this.StartCoroutine(added);
		});
	}

	public void Add(Effect effect)
	{
		EffectStack stack = this.GetOrCreateStack(effect.tag);
		Finalizable effectRoutine = this.effectRoutineCreator.Create(effect, out Action revert);

		this.StoreEffect(effect, revert, effectRoutine, stack);
		this.StackEffectRoutine(effectRoutine, stack);
	}

	public void Cancel(EffectTag tag)
	{
		if (this.stacks.TryGetValue(tag, out EffectStack stack)) {
			stack.routines.ForEach(r => this.StopCoroutine(r));
			stack.routines.Clear();
			stack.effects.ForEach(e => e.revert?.Invoke());
			stack.effects.Clear();
		}
	}
}
