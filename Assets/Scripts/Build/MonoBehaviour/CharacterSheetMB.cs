using System;
using UnityEngine;

[RequireComponent(typeof(IntensityManagerMB), typeof(DurationManagerMB))]
public class CharacterSheetMB : MonoBehaviour, IConditionManager
{
	private IntensityManagerMB stackIntensity;
	private DurationManagerMB stackDuration;

	public void Add(Effect effect)
	{
		IConditionManager manager = effect.stacking switch {
			ConditionStacking.Duration => this.stackDuration,
			ConditionStacking.Intensity => this.stackIntensity,
			_ => throw new ArgumentException($"Invalid stacking {effect.stacking} for {this.name} (CharacterSheetMB)"),
		};
		manager.Add(effect);
	}

	private void Awake()
	{
		this.stackIntensity = this.GetComponent<IntensityManagerMB>();
		this.stackDuration = this.GetComponent<DurationManagerMB>();
	}
}
