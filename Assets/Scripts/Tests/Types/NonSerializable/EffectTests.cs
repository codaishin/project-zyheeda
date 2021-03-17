using System;
using NUnit.Framework;

public class EffectTests : TestCollection
{
	[Test]
	public void Apply()
	{
		var called = false;
		var effect = new Effect((out Action r) => {
			called = true;
			r = default;
		});

		effect.Apply(out _);

		Assert.True(called);
	}

	[Test]
	public void DefaultApplyDoesNotThrow()
	{
		var effect = new Effect();

		Assert.DoesNotThrow(() => effect.Apply(out _));
	}

	[Test]
	public void Maintain()
	{
		var called = 0f;
		var effect = new Effect(maintain: d => called = d);

		effect.Apply(out _);
		effect.Maintain(0.4f);

		Assert.AreEqual(0.4f, called);
	}

	[Test]
	public void DefaultMaintainDoesNotThrow()
	{
		var effect = new Effect();

		Assert.DoesNotThrow(() => effect.Maintain(0.4f));
	}

	[Test]
	public void MaintainReducesDuration()
	{
		var effect = new Effect();
		effect.duration = 5f;

		effect.Apply(out _);
		effect.Maintain(3f);

		Assert.AreEqual(2f, effect.duration);
	}

	[Test]
	public void Revert()
	{
		var called = false;
		var effect = new Effect((out Action r) => r = () => called = true);

		var outed = effect.Apply(out var revert);
		revert();

		Assert.AreEqual((true, true), (outed, called));
	}

	[Test]
	public void FalseApplyWhenNoRevert()
	{
		var effect = new Effect((out Action r) => r = default);

		Assert.False(effect.Apply(out _));
	}

	[Test]
	public void SilenceApplyAndRevert()
	{
		var called = false;
		var effect = new Effect((out Action r) => {
			called = true;
			r = () => {};
		});
		effect.silence = SilenceTag.ApplyAndRevert;

		var outed = effect.Apply(out _);

		Assert.AreEqual((false, false), (outed, called));
	}


	[Test]
	public void SilenceMaintain()
	{
		var called = false;
		var effect = new Effect(maintain: d => called = true);
		effect.silence = SilenceTag.Maintain;

		effect.Maintain(2);

		Assert.False(called);
	}

	[Test]
	public void SilenceMaintainReduceDuration()
	{
		var effect = new Effect();
		effect.silence = SilenceTag.Maintain;
		effect.duration = 4f;

		effect.Maintain(2);

		Assert.AreEqual(4f - 2f, effect.duration);
	}
}
