using System;
using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionManager, ISections
{
	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;

	public Health health;

	public void Add(Effect condition)
	{
		IConditionManager stack = condition.stacking switch {
			ConditionStacking.Duration => this.stackDuration,
			ConditionStacking.Intensity => this.stackIntensity,
			_ => throw new ArgumentException($"Invalid stacking {condition.stacking} for {this}"),
		};
		stack.Add(condition);
	}

	public bool UseSection<TSection>(RefAction<TSection> action)
	{
		bool success = true;
		Action run = action switch {
			RefAction<Health> call => () => call(ref this.health),
			_ => () => success = false,
		};
		run();
		return success;
	}

	private void Awake()
	{
		this.stackIntensity = this.GetComponent<IntensityManagerMB>();
		this.stackDuration = this.GetComponent<DurationManagerMB>();
	}
}
