using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffectRunnerMB<TEffectRoutineFactory> : MonoBehaviour, IEffectRunner
	where TEffectRoutineFactory : IEffectRoutineFactory
{
	private Dictionary<EffectTag, Dictionary<ConditionStacking, IStack>> stacksMap =
		new Dictionary<EffectTag, Dictionary<ConditionStacking, IStack>>();
	public TEffectRoutineFactory routineFactory;

	public void Push(Effect effect)
	{
		if (!this.stacksMap.TryGetValue(effect.tag, out Dictionary<ConditionStacking, IStack> stacks)) {
			stacks = new Dictionary<ConditionStacking, IStack>();
			this.stacksMap[effect.tag] = stacks;
		}
		if (!stacks.TryGetValue(effect.stacking, out IStack stack)) {
			stack = this.GetStack(effect.stacking)(
				effectToRoutine: this.routineFactory.Create,
				onPull: this.StartEffect,
				onCancel: this.CancelEffect
			);
			stacks[effect.stacking] = stack;
		}
		stack.Push(effect);
	}

	public abstract GetStackFunc GetStack(ConditionStacking stacking);

	private void StartEffect(Finalizable routine)
	{
		this.StartCoroutine(routine);
	}

	private void CancelEffect(Finalizable routine)
	{
		this.StopCoroutine(routine);
	}
}
