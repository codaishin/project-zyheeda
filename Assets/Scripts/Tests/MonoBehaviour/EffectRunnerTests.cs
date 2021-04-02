using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EffectRunnerTests : TestCollection
{
	[Test]
	public void GetIntensityStack()
	{
		var runner = new GameObject("runner").AddComponent<EffectRunnerMB>();

		Assert.True(runner.GetStack(ConditionStacking.Intensity) == IntensityStackFactory.Create);
	}

	[Test]
	public void GetDurationStack()
	{
		var runner = new GameObject("runner").AddComponent<EffectRunnerMB>();

		Assert.True(runner.GetStack(ConditionStacking.Duration) == DurationStackFactory.Create);
	}

	[Test]
	public void ArgumentException()
	{
		var runner = new GameObject("runner").AddComponent<EffectRunnerMB>();

		Assert.Throws<ArgumentException>(() => runner.GetStack((ConditionStacking)(-1)));
	}

	[Test]
	public void ArgumentExceptionMessage()
	{
		var runner = new GameObject("runner").AddComponent<EffectRunnerMB>();

		try {
			runner.GetStack((ConditionStacking)(-1));
		} catch (ArgumentException e) {
			Assert.AreEqual("Stacking type -1 is not configured on runner (EffectRunnerMB)", e.Message);
		}
	}
}
