using System;
using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionManager, ISections
{
	private const string stackingExcFmt = "Invalid stacking {0} for {1}";
	private const string requireExcFmt = "{0} is not a valid section for {1}";

	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;

	private ArgumentException Error<T>(string fmt, T value) => new ArgumentException(string.Format(fmt, value, this));

	public void Add(Effect condition)
	{
		IConditionManager stack = condition.stacking switch {
			ConditionStacking.Duration => this.stackDuration,
			ConditionStacking.Intensity => this.stackIntensity,
			_ => throw this.Error(CharacterSheetMB.stackingExcFmt, condition.stacking),
		};
		stack.Add(condition);
	}

	public void UseSection<TSection>(SectionAction<TSection> action, bool required)
	{
		Action run = action switch {
			_ when required => () => throw this.Error(CharacterSheetMB.requireExcFmt, typeof(TSection)),
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
