using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionManager, ISections
{
	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;
	private AsDictWrapper<EffectTag, float> resistanceDict;

	public Health health;
	public List<Record<EffectTag, float>> resistance;

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
			RefAction<AsDictWrapper<EffectTag, float>> use => () => use(ref this.resistanceDict),
			_ => fallback,
		};
	}

	private void Awake()
	{
		this.stackIntensity = this.GetComponent<IntensityManagerMB>();
		this.stackDuration = this.GetComponent<DurationManagerMB>();
		if (this.resistance == null) {
			this.resistance = new List<Record<EffectTag, float>>();
			this.resistanceDict = new AsDictWrapper<EffectTag, float>(this.resistance);
		}
	}
}
