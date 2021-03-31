using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffectRunnerMB<TEffectRoutineFactory, TStackFactory> : MonoBehaviour
	where TEffectRoutineFactory : IEffectRoutineFactory
	where TStackFactory : IStackFactory
{
	private Dictionary<ConditionStacking, IStack> stacks = new Dictionary<ConditionStacking, IStack>();

	public TEffectRoutineFactory routineFactory;
	public Record<ConditionStacking, TStackFactory>[] records;

	public IStack this[ConditionStacking stacking] => stacks[stacking];

	private void Start()
	{
		this.ValidateRecords();
		this.UpdateStacks();
	}

	public void OnValidate()
	{
		if (this.records != null) {
			this.ValidateRecords();
			this.UpdateStacks();
		}
	}

	private void ValidateRecords()
	{
		this.records = this.records
			.Validate()
			.Select(this.MarkUnset)
			.ToArray();
	}

	private void UpdateStacks()
	{
		this.records
			.Where(this.RecordIsValid)
			.ForEach(this.UpdateStack);
	}

	private void OnPull(Finalizable routine)
	{
		this.StartCoroutine(routine);
	}

	private void OnCancel(Finalizable routine)
	{
		this.StopCoroutine(routine);
	}

	private void UpdateStack(Record<ConditionStacking, TStackFactory> record)
	{
		this.stacks[record.key] = record.value.Create(this.routineFactory.Create, this.OnPull, this.OnCancel);
	}

	private bool RecordIsValid(Record<ConditionStacking, TStackFactory> record)
	{
		return record.name == record.key.ToString();
	}

	private Record<ConditionStacking, TStackFactory> MarkUnset(Record<ConditionStacking, TStackFactory> record) {
		if (record.value == null) {
			record.name = "__no_factory_set__";
		}
		return record;
	}
}
