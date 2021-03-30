using System;
using NUnit.Framework;

public class EffectTests : TestCollection
{
	[Test]
	public void Apply()
	{
		var called = false;
		var effect = new Effect(() => called = true);

		effect.Apply();

		Assert.True(called);
	}

	[Test]
	public void DefaultApplyDoesNotThrow()
	{
		var effect = new Effect();

		Assert.DoesNotThrow(() => effect.Apply());
	}

	[Test]
	public void Maintain()
	{
		var called = 0f;
		var effect = new Effect(maintain: d => called = d);

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

		effect.Maintain(3f);

		Assert.AreEqual(2f, effect.duration);
	}

	[Test]
	public void Revert()
	{
		var called = false;
		var effect = new Effect(revert: () => called = true);

		effect.Revert();

		Assert.True(called);
	}

	[Test]
	public void DefaultRevertDoesNotThrow()
	{
		var effect = new Effect();

		Assert.DoesNotThrow(() => effect.Revert());
	}

	[Test]
	public void SilenceApplyAndRevert()
	{
		var called = (apply: false, revert: false);
		var effect = new Effect(
			apply: () => called.apply = true,
			revert: () => called.revert = true
		);
		effect.silence = SilenceTag.ApplyAndRevert;

		effect.Apply();
		effect.Revert();

		Assert.AreEqual((false, false), called);
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
