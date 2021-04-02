using System.Collections.Generic;

public class EffectRunnerMB : BaseEffectRunnerMB<EffectRoutineFactory>
{
	protected override
	Dictionary<ConditionStacking, GetStackFunc> Factories { get; } = new Dictionary<ConditionStacking, GetStackFunc> {
		{ ConditionStacking.Intensity, IntensityStackFactory.Create },
		{ ConditionStacking.Duration, DurationStackFactory.Create },
	};
}
