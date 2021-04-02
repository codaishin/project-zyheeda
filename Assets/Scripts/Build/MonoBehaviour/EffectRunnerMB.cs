using System;

public class EffectRunnerMB : BaseEffectRunnerMB<EffectRoutineFactory>
{
	public override GetStackFunc GetStack(ConditionStacking stacking) => stacking switch {
		ConditionStacking.Intensity => IntensityStackFactory.Create,
		ConditionStacking.Duration => DurationStackFactory.Create,
		_ => throw new ArgumentException($"Stacking type {stacking} is not configured on {this}"),
	};
}
