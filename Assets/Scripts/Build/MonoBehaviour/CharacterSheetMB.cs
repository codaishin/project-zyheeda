using System;
using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionManager, ISections
{
	private const string stackingErrFmt = "Invalid stacking {0} for {1}";
	private const string sectionErrFmt = "{0} is not a valid section for {1}";

	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;

	public Health health;

	private ArgumentException Error<T>(string fmt, T value) => new ArgumentException(string.Format(fmt, value, this));

	public void Add(Effect condition)
	{
		IConditionManager stack = condition.stacking switch {
			ConditionStacking.Duration => this.stackDuration,
			ConditionStacking.Intensity => this.stackIntensity,
			_ => throw this.Error(CharacterSheetMB.stackingErrFmt, condition.stacking),
		};
		stack.Add(condition);
	}

	public void UseSection<TSection>(SectionAction<TSection> action, bool required)
	{
		Action run = action switch {
			SectionAction<Health> a => () => a(ref this.health),
			_ when required => () => throw this.Error(CharacterSheetMB.sectionErrFmt, typeof(TSection)),
			_ => () => { },
		};
		run();
	}

	private void Awake()
	{
		this.stackIntensity = this.GetComponent<IntensityManagerMB>();
		this.stackDuration = this.GetComponent<DurationManagerMB>();
	}
}
