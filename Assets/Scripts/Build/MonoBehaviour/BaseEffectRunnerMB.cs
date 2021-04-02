using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffectRunnerMB<TEffectRoutineFactory> : MonoBehaviour
	where TEffectRoutineFactory : IEffectRoutineFactory
{
	private Dictionary<EffectTag, Dictionary<ConditionStacking, IStack>> stacksMap =
		new Dictionary<EffectTag, Dictionary<ConditionStacking, IStack>>();
	public TEffectRoutineFactory routineFactory;

	public IStack this[EffectTag tag, ConditionStacking stacking] => this.GetOrMakeStack(tag, stacking);

	public abstract GetStackFunc GetStack(ConditionStacking stacking);

	private IStack GetOrMakeStack(EffectTag tag, ConditionStacking stacking)
	{
		if (!this.stacksMap.TryGetValue(tag, out Dictionary<ConditionStacking, IStack> stacks)) {
			stacks = new Dictionary<ConditionStacking, IStack>();
			this.stacksMap[tag] = stacks;
		}
		if (!stacks.TryGetValue(stacking, out IStack stack)) {
			stack = this.GetStack(stacking)(
				effectToRoutine: this.routineFactory.Create,
				onPull: this.StartEffect,
				onCancel: this.CancelEffect
			);
			stacks[stacking] = stack;
		}
		return stack;
	}

	private void StartEffect(Finalizable routine)
	{
		this.StartCoroutine(routine);
	}

	private void CancelEffect(Finalizable routine)
	{
		this.StopCoroutine(routine);
	}
}
