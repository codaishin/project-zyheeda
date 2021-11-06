using UnityEngine;
using System;
using Stack = System.Collections.Generic.Dictionary<
	ConditionStacking,
	IStack
>;
using EffectToStackMap = System.Collections.Generic.Dictionary<
	EffectTag,
	System.Collections.Generic.Dictionary<ConditionStacking, IStack>
>;

public abstract class BaseEffectRunnerMB<TEffectRoutineFactory> :
	MonoBehaviour,
	IEffectRunner
	where TEffectRoutineFactory :
		IEffectRoutineFactory
{
	private EffectToStackMap stacksMap = new EffectToStackMap();
	private Func<Effect, Finalizable>? effectToRoutine;
	public TEffectRoutineFactory? routineFactory;

	private Func<Effect, Finalizable> EffectToRoutine
		=> this.effectToRoutine ?? (this.effectToRoutine = this.FactoryCreate());

	public void Push(Effect effect) {
		if (!this.stacksMap.TryGetValue(effect.tag, out Stack stacks)) {
			stacks = new Stack();
			this.stacksMap[effect.tag] = stacks;
		}
		if (!stacks.TryGetValue(effect.stacking, out IStack stack)) {
			stack = this.GetStack(effect.stacking)(
				effectToRoutine: this.EffectToRoutine,
				onPull: this.StartEffect,
				onCancel: this.CancelEffect
			);
			stacks[effect.stacking] = stack;
		}
		stack.Push(effect);
	}

	public abstract GetStackFunc GetStack(ConditionStacking stacking);

	private Func<Effect, Finalizable> FactoryCreate() {
		if (this.routineFactory == null) throw this.NullError();
		return this.routineFactory.Create;
	}

	private void StartEffect(Finalizable routine) {
		this.StartCoroutine(routine);
	}

	private void CancelEffect(Finalizable routine) {
		this.StopCoroutine(routine);
	}
}
