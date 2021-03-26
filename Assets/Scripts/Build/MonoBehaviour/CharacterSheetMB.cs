using System;
using UnityEngine;

[RequireComponent(typeof(ResistanceMB))]
[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionManager, ISections
{
	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;
	private Resistance resistance;

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
	}

	private void Start()
	{
		this.resistance = this.GetComponent<ResistanceMB>().Resistance;
	}
}
