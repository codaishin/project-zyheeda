using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EffectTests : TestCollection
{
	[Test]
	public void OnApply()
	{
		var called = false;
		var effect = new Effect();
		effect.OnApply += () => called = true;

		effect.Apply();

		Assert.True(called);
	}

	[Test]
	public void OnApplyDoesNotThrow()
	{
		var effect = new Effect();

		Assert.DoesNotThrow(() => effect.Apply());
	}

	[Test]
	public void OnMaintain()
	{
		var called = 0f;
		var effect = new Effect();
		effect.OnMaintain += d => called = d;

		effect.Apply();
		effect.Maintain(0.4f);

		Assert.AreEqual(0.4f, called);
	}

	[Test]
	public void OnMaintainDoesNotThrow()
	{
		var effect = new Effect();

		effect.Apply();
		Assert.DoesNotThrow(() => effect.Maintain(0.4f));
	}

	[Test]
	public void OnMaintainReducesDuration()
	{
		var effect = new Effect();
		effect.duration = 5f;

		effect.Apply();
		effect.Maintain(3f);

		Assert.AreEqual(2f, effect.duration);
	}

	[Test]
	public void OnRevert()
	{
		var called = false;
		var effect = new Effect();
		effect.OnRevert += () => called = true;

		effect.Apply();
		effect.Revert();

		Assert.True(called);
	}

	[Test]
	public void OnRevertDoesNotThrow()
	{
		var effect = new Effect();

		effect.Apply();
		Assert.DoesNotThrow(() => effect.Revert());
	}

	[Test]
	public void NoRevertWhenNoApply()
	{
		var called = false;
		var effect = new Effect();
		effect.OnRevert += () => called = true;

		effect.Revert();

		Assert.False(called);
	}

	[Test]
	public void NoMaintainWhenNoApply()
	{
		var called = false;
		var effect = new Effect();
		effect.OnMaintain += d => called = true;

		effect.Maintain(2);

		Assert.False(called);
	}

	[Test]
	public void NoApplyWhenReverted()
	{
		var called = false;
		var effect = new Effect();

		effect.Apply();
		effect.Revert();
		effect.OnApply += () => called = true;
		effect.Apply();

		Assert.False(called);
	}
}
