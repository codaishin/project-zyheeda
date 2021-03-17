using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EffectRoutineCreatorTests : TestCollection
{
	[Test]
	public void CreateAndApply()
	{
		var called = false;
		var effect = new Effect((out Action r) => {
			called = true;
			r = () => {};
		});
		var routine = new EffectRoutineCreator{ intervalDelta = 0f }.Create(effect, out _);

		routine.MoveNext();

		Assert.True(called);
	}

	[Test]
	public void CreateAndMaintain()
	{
		var called = 0f;
		var effect = new Effect((out Action r) => r = () => {}, d => called = d);
		var routine = new EffectRoutineCreator{ intervalDelta = 3f }.Create(effect, out _);

		effect.duration = 10f;

		routine.MoveNext();
		routine.MoveNext();

		Assert.AreEqual(3f, called);
	}

	[Test]
	public void CreateAndNoMaintain()
	{
		var called = false;
		var effect = new Effect((out Action r) => r = () => {}, maintain: _ => called = true);
		var routine = new EffectRoutineCreator{ intervalDelta = 3f }.Create(effect, out _);

		routine.MoveNext();

		Assert.AreEqual(false, called);
	}

	[Test]
	public void CreateAndMaintainForDuration()
	{
		var called = new List<float>();
		var effect = new Effect((out Action r) => r = () => {}, d => called.Add(d));
		var routine = new EffectRoutineCreator{ intervalDelta = 2f }.Create(effect, out _);

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
		var routine = new EffectRoutineCreator{ intervalDelta = 0.3f }.Create(effect, out _);

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
		var routine = new EffectRoutineCreator{ intervalDelta = 0.3f }.Create(effect, out _);

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
		var effect = new Effect((out Action r) => r = () => called = true);
		var routine = new EffectRoutineCreator{ intervalDelta = 0f }.Create(effect, out _);

		routine.MoveNext();

		Assert.True(called);
	}

	[Test]
	public void CreateAndRevertDoesNotThrow()
	{
		var effect = new Effect((out Action r) => r = default);
		var routine = new EffectRoutineCreator{ intervalDelta = 0f }.Create(effect, out _);

		Assert.DoesNotThrow(() => routine.MoveNext());
	}

	[Test]
	public void CreateAndRevertOnlyAtEnd()
	{
		var called = false;
		var calledTrack = new List<bool>();
		var effect = new Effect((out Action r) => r = () => called = true);
		var routine = new EffectRoutineCreator{ intervalDelta = 42f }.Create(effect, out _);

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
		var routine = new EffectRoutineCreator();
		Assert.AreEqual(1f, routine.intervalDelta);
	}

	[Test]
	public void PassRevertOut()
	{
		var called = false;
		var calledTrack = new List<bool>();
		var effect = new Effect((out Action r) => r = () => called = true){ duration = 1f };
		var routine = new EffectRoutineCreator().Create(effect, out var revert);

		effect.duration = 42f;

		routine.MoveNext();
		revert();

		Assert.True(called);
	}

	[Test]
	public void PassefRevertDoesNotThrow()
	{
		var calledTrack = new List<bool>();
		var effect = new Effect((out Action r) => r = default){ duration = 1f };
		var routine = new EffectRoutineCreator().Create(effect, out var revert);

		effect.duration = 42f;

		routine.MoveNext();
		Assert.DoesNotThrow(() => revert());
	}

	[Test]
	public void PassefRevertDoesNotThrowWhenNotApplied()
	{
		var calledTrack = new List<bool>();
		var effect = new Effect((out Action r) => r = default){ duration = 1f };
		var routine = new EffectRoutineCreator().Create(effect, out var revert);

		Assert.DoesNotThrow(() => revert());
	}
}
