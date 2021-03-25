using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionManager, ISections
{
	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;
	private Resistance resistance;

	public Health health;
	public List<Record<EffectTag, float>> resistanceRecords;

	public void Add(Effect condition)
	{
		IConditionManager stack = condition.stacking switch {
			ConditionStacking.Duration => this.stackDuration,
			ConditionStacking.Intensity => this.stackIntensity,
			_ => throw new ArgumentException($"Invalid stacking {condition.stacking} for {this}"),
		};
		stack.Add(condition);
	}

	public Action UseSection<TSection>(RefAction<TSection> action, Action fallback)
	{
		return action switch {
			RefAction<Health> use => () => use(ref this.health),
			RefAction<Resistance> use => () => use(ref this.resistance),
			_ => fallback,
		};
	}

	private void Awake()
	{
		this.stackIntensity = this.GetComponent<IntensityManagerMB>();
		this.stackDuration = this.GetComponent<DurationManagerMB>();
		if (this.resistanceRecords == null) {
			this.resistanceRecords = new List<Record<EffectTag, float>>();
		}
		this.resistance = new Resistance(this.resistanceRecords);
	}

	private void OnValidate()
	{
		this.resistanceRecords.Validate();
	}
}
