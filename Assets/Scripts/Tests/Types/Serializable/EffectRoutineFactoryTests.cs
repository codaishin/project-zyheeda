using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EffectRoutineFactoryTests : TestCollection
{
	[Test]
	public void CreateAndApply()
	{
		var called = false;
		var effect = new Effect(() => called = true);
		var routine = new EffectRoutineFactory{ intervalDelta = 0f }.Create(effect);

		routine.MoveNext();

		Assert.True(called);
	}

	[Test]
	public void CreateAndMaintain()
	{
		var called = 0f;
		var effect = new Effect(maintain: d => called = d);
		var routine = new EffectRoutineFactory{ intervalDelta = 3f }.Create(effect);

		effect.duration = 10f;

		routine.MoveNext();
		routine.MoveNext();

		Assert.AreEqual(3f, called);
	}

	[Test]
	public void CreateAndNoMaintainWhenZeroDuration()
	{
		var called = false;
		var effect = new Effect(maintain: _ => called = true);
		var routine = new EffectRoutineFactory{ intervalDelta = 3f }.Create(effect);

		routine.MoveNext();

		Assert.AreEqual(false, called);
	}

	[Test]
	public void CreateAndMaintainForDuration()
	{
		var called = new List<float>();
		var effect = new Effect(maintain: d => called.Add(d));
		var routine = new EffectRoutineFactory{ intervalDelta = 2f }.Create(effect);

		effect.duration = 5f;

		routine.MoveNext();  // Apply
		routine.MoveNext();  // Maintain
		routine.MoveNext();  // Maintain
		routine.MoveNext();  // Maintain

		Assert.AreEqual(new float[] { 2f, 2f, 1f }, called);
	}

	[UnityTest]
	public IEnumerator CreateAndWaitForIntervalDelta()
	{
		var effect = new Effect();
		var routine = new EffectRoutineFactory{ intervalDelta = 0.3f }.Create(effect);

		effect.duration = 10f;
		routine.MoveNext();

		float before = Time.time;

		yield return routine.Current;

		float after = Time.time;

		Assert.AreEqual(0.3f, after - before, 0.01f);
	}

	[UnityTest]
	public IEnumerator CreateAndWaitForRemainingCooldown()
	{
		var effect = new Effect();
		var routine = new EffectRoutineFactory{ intervalDelta = 0.3f }.Create(effect);

		effect.duration = 0.4f;
		routine.MoveNext();
		routine.MoveNext();

		float before = Time.time;

		yield return routine.Current;

		float after = Time.time;

		Assert.AreEqual(0.1f, after - before, 0.01f);
	}

	[Test]
	public void CreateAndRevert()
	{
		var called = false;
		var effect = new Effect(revert: () => called = true);
		var routine = new EffectRoutineFactory{ intervalDelta = 0f }.Create(effect);

		routine.MoveNext();

		Assert.True(called);
	}

	[Test]
	public void CreateAndRevertOnlyAtEnd()
	{
		var called = false;
		var calledTrack = new List<bool>();
		var effect = new Effect(revert: () => called = true);
		var routine = new EffectRoutineFactory{ intervalDelta = 42f }.Create(effect);

		effect.duration = 42f;

		routine.MoveNext();
		calledTrack.Add(called);
		routine.MoveNext();
		calledTrack.Add(called);

		CollectionAssert.AreEqual(new bool[] { false, true }, calledTrack);
	}

	[Test]
	public void IntervalDeltaDefaultToOne()
	{
		var routine = new EffectRoutineFactory();
		Assert.AreEqual(1f, routine.intervalDelta);
	}

	[Test]
	public void IntervalDeltaUntouched()
	{
		var factory = new EffectRoutineFactory{ intervalDelta = 0.1f };
		var routine = factory.Create(new Effect{ duration = 10f });
		var count = 0;

		while (routine.MoveNext() && count++ < 100);

		Assert.AreEqual((0.1f, 100), (factory.intervalDelta, count));
	}
}
