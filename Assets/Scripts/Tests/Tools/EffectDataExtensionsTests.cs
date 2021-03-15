using System;
using NUnit.Framework;
using UnityEngine;

public class EffectDataExtensionsTests : TestCollection
{
	private class MockSheet : ISections
	{
		public bool UseSection<TSection>(RefAction<TSection> action) => false;
	}

	private class MockEffectBehaviourSO : BaseEffectBehaviourSO
	{
		public Action<ISections, ISections, int> apply = (s, t, i) => { };
		public Action<ISections, ISections, int, float> maintain = (s, t, i, d) => { };
		public Action<ISections, ISections, int> revert = (s, t, i) => { };

		public override
		void Apply<T>(T source, T target, int intensity) =>
			this.apply(source, target, intensity);

		public override
		void Maintain<T>(T source, T target, int intensity, float intervalDelta) =>
			this.maintain(source, target, intensity, intervalDelta);

		public override
		void Revert<T>(T source, T target, int intensity) =>
			this.revert(source, target, intensity);
	}

	[Test]
	public void CreateEffectApply()
	{
		var called = default((MockSheet, MockSheet));
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };
		behaviour.apply = (s, t, _) => called = (s as MockSheet, t as MockSheet);

		var effect = data.GetEffect(source, target);
		effect.Apply();

		Assert.AreEqual((source, target), called);
	}

	[Test]
	public void CreateEffectMaintain()
	{
		var called = default((MockSheet, MockSheet, float));
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };
		behaviour.maintain = (s, t, _, d) => called = (s as MockSheet, t as MockSheet, d);

		var effect = data.GetEffect(source, target);
		effect.duration = 1f;
		effect.Apply();
		effect.Maintain(0.42f);

		Assert.AreEqual((source, target, 0.42f), called);
	}

	[Test]
	public void CreateEffectRevert()
	{
		var called = default((MockSheet, MockSheet));
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };
		behaviour.revert = (s, t, _) => called = (s as MockSheet, t as MockSheet);

		var effect = data.GetEffect(source, target);
		effect.Apply();
		effect.Revert();

		Assert.AreEqual((source, target), called);
	}

	[Test]
	public void SetDuration()
	{
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour, duration = 42f };

		var effect = data.GetEffect(source, target);

		Assert.AreEqual(42f, effect.duration);
	}

	[Test]
	public void UseIntensity()
	{
		var called = (a: 0, m: 0, r: 0);
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour, duration = 1, intensity = 7 };

		behaviour.apply = (_, __, i) => called.a = i;
		behaviour.maintain = (_, __, i, ___) => called.m = i;
		behaviour.revert = (_, __, i) => called.r = i;

		var effect = data.GetEffect(source, target);

		effect.Apply();
		effect.Maintain(5f);
		effect.Revert();

		Assert.AreEqual((7, 7, 7), called);
	}

	[Test]
	public void ApplySilenced()
	{
		var called = false;
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };

		behaviour.apply = (_, __, ___) => called = true;
		data.silence = SilenceTag.ApplyAndRevert;

		var effect = data.GetEffect(source, target);

		effect.Apply();

		Assert.False(called);
	}

	[Test]
	public void RevertSilencedWhenApplySilenced()
	{
		var called = false;
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };

		behaviour.revert = (_, __, ___) => called = true;
		data.silence = SilenceTag.ApplyAndRevert;

		var effect = data.GetEffect(source, target);

		effect.Apply();
		effect.Revert();

		Assert.False(called);
	}

	[Test]
	public void MaintainSilenced()
	{
		var called = false;
		var source = new MockSheet();
		var target = new MockSheet();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };

		behaviour.maintain = (_, __, ___, ____) => called = true;
		data.silence = SilenceTag.Maintain;

		var effect = data.GetEffect(source, target);

		effect.Apply();
		effect.Maintain(0.1f);

		Assert.False(called);
	}
}
