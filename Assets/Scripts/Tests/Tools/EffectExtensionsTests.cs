using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EffectExtensionsTests : TestCollection
{
	[Test]
	public void AsTimedRoutineApply()
	{
		var called = false;
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(0);

		effect.OnApply += () => called = true;

		routine.MoveNext();

		Assert.True(called);
	}

	[Test]
	public void AsTimedRoutineMaintain()
	{
		var called = 0f;
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(3f);

		effect.duration = 10f;
		effect.OnMaintain += d => called = d;

		routine.MoveNext();
		routine.MoveNext();

		Assert.AreEqual(3f, called);
	}

	[Test]
	public void AsTimedRoutineNoMaintain()
	{
		var called = false;
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(3f);

		effect.OnMaintain += _ => called = true;

		routine.MoveNext();

		Assert.AreEqual(false, called);
	}

	[Test]
	public void AsTimedRoutineMaintainForDuration()
	{
		var called = new List<float>();
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(2f);

		effect.OnMaintain += d => called.Add(d);
		effect.duration = 5f;

		routine.MoveNext();  // Apply
		routine.MoveNext();  // Maintain
		routine.MoveNext();  // Maintain
		routine.MoveNext();  // Maintain

		Assert.AreEqual(new float[] { 2f, 2f, 1f }, called);
	}

	[UnityTest]
	public IEnumerator AsTimedRoutineWaitForIntervalDelta()
	{
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(0.3f);

		effect.duration = 10f;
		routine.MoveNext();

		float before = Time.time;

		yield return routine.Current;

		float after = Time.time;

		Assert.AreEqual(0.3f, after - before, 0.01f);
	}

	[UnityTest]
	public IEnumerator AsTimedRoutineWaitForRemainingCooldown()
	{
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(0.3f);

		effect.duration = 0.4f;
		routine.MoveNext();
		routine.MoveNext();

		float before = Time.time;

		yield return routine.Current;

		float after = Time.time;

		Assert.AreEqual(0.1f, after - before, 0.01f);
	}

	[Test]
	public void AsTimedRoutineRevert()
	{
		var called = false;
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(0);

		effect.OnRevert += () => called = true;

		routine.MoveNext();

		Assert.True(called);
	}

	[Test]
	public void AsTimedRoutineRevertOnlyAtEnd()
	{
		var called = false;
		var calledTrack = new List<bool>();
		var effect = new Effect();
		var routine = effect.AsTimedRoutine(42f);

		effect.duration = 42f;
		effect.OnRevert += () => called = true;

		routine.MoveNext();
		calledTrack.Add(called);
		routine.MoveNext();
		calledTrack.Add(called);

		CollectionAssert.AreEqual(new bool[] { false, true }, calledTrack);
	}
}
