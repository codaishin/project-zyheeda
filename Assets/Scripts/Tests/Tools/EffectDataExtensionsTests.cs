using System;
using NUnit.Framework;
using UnityEngine;

public class EffectDataExtensionsTests : TestCollection
{
	private class MockSheet : ISections
	{
		public Action UseSection<TSection>(RefAction<TSection> action, Action fallback) => fallback;
	}

	private class MockEffectFactory : IEffectFactory
	{
		public Func<ISections, ISections, float, Effect> create = (_, __, ___) => new Effect();

		public Effect Create<TSheet>(TSheet s, TSheet t, float i)
			where TSheet : ISections => this.create(s, t, i);

	}

	[Test]
	public void CreateEffectThroughFactory()
	{
		var effect = new Effect();
		var factory = new MockEffectFactory();
		var data = new EffectData<MockSheet, MockEffectFactory> { factory = factory };
		factory.create = (_, __, ___) => effect;

		Assert.AreSame(effect, data.GetEffect(default, default));
	}

	[Test]
	public void CreateEffectWithParameters()
	{
		var called = (default(MockSheet), default(MockSheet), 0f);
		var factory = new MockEffectFactory();
		var source = new MockSheet();
		var target = new MockSheet();
		var data = new EffectData<MockSheet, MockEffectFactory> { factory = factory, intensity = 4000000f };
		factory.create = (s, t, i) => {
			called = ((MockSheet)s, (MockSheet)t, i);
			return new Effect();
		};

		data.GetEffect(source, target);

		Assert.AreEqual((source, target, 4000000f), called);
	}

	[Test]
	public void CreateEffectWithData()
	{
		var factory = new MockEffectFactory();
		var source = new MockSheet();
		var target = new MockSheet();
		var data = new EffectData<MockSheet, MockEffectFactory> { factory = factory, silence = SilenceTag.Maintain, duration = 10f };

		var effect = data.GetEffect(source, target);

		Assert.AreEqual((SilenceTag.Maintain, 10f), (effect.silence, effect.duration));
	}
}
