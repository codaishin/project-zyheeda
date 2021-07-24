using System;
using NUnit.Framework;
using UnityEngine;

public class BaseEffectCollectionTests : TestCollection
{
	private class MockEffectRunner : IEffectRunner
	{
		public Action<Effect> push;
		public void Push(Effect effect) => this.push(effect);
	}

	private class MockSheetMB : MonoBehaviour, ISections
	{
		public MockEffectRunner runner = new MockEffectRunner();

		public Action UseSection<T>(RefAction<T> action, Action fallback) {
			IEffectRunner runner = this.runner;
			return action switch {
				RefAction<IEffectRunner> use => () => use(ref runner),
				_ => null,
			};
		}
	}

	private class MockEffectFactory : IEffectFactory
	{
		public Func<ISections, ISections, float, Effect> create = (_, __, ___) => new Effect();

		public Effect Create<TSheet>(TSheet s, TSheet t, float i)
			where TSheet : ISections => this.create(s, t, i);
	}

	private class MockEffectCollection : BaseEffectCollection<MockSheetMB, MockEffectFactory> {}


	[Test]
	public void ApplyUsesEffectApply()
	{
		var called = (default(MockSheetMB), default(MockSheetMB), 0f);
		var coll = new MockEffectCollection();
		var factory = new MockEffectFactory();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		factory.create = (s, t, i) => new Effect(() => called = ((MockSheetMB)s, (MockSheetMB)t, i));
		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory, intensity = 4 }
		};

		coll.Apply(source, target);

		Assert.AreEqual((source, target, 4f), called);
	}

	[Test]
	public void ApplyUsesEffectReverse()
	{
		var called = false;
		var coll = new MockEffectCollection();
		var factory = new MockEffectFactory();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		factory.create = (s, t, i) => new Effect(revert: () => called = true);
		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory }
		};

		coll.Apply(source, target);

		Assert.True(called);
	}

	[Test]
	public void UseTargetAdd()
	{
		var called = default(Effect);
		var coll = new MockEffectCollection();
		var factory = new MockEffectFactory();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		var effect = new Effect();

		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory, duration = 1f }
		};

		factory.create = (_, __, ___) => effect;
		target.runner.push = e => called = e;

		coll.Apply(source, target);

		Assert.AreSame(effect, called);
	}
}
