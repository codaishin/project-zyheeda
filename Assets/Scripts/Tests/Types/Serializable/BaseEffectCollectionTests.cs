using System;
using NUnit.Framework;
using UnityEngine;

public class BaseEffectCollectionTests : TestCollection
{
	private class MockSheetMB : MonoBehaviour, IConditionManager, ISections
	{
		public Action<Effect> add = (_) => { };

		public void Add(Effect effect) => this.add(effect);
		public Action UseSection<T>(RefAction<T> action, Action fallback) => fallback;
	}

	private class MockEffectFactory : IEffectFactory<MockSheetMB>
	{
		public Func<MockSheetMB, MockSheetMB, float, Effect> create = (_, __, ___) => new Effect();

		public Effect Create(MockSheetMB s, MockSheetMB t, float i) => this.create(s, t, i);
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

		factory.create = (s, t, i) => new Effect((out Action r) => {
			called = (s, t, i);
			r = () => {};
		});
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

		factory.create = (s, t, i) => new Effect((out Action r) => r = () => called = true);
		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory }
		};

		coll.Apply(source, target);

		Assert.True(called);
	}

	[Test]
	public void ApplyEffectDefaultRevertDoesNotThrow()
	{
		var coll = new MockEffectCollection();
		var factory = new MockEffectFactory();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		factory.create = (s, t, i) => new Effect((out Action r) => r = default);
		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory }
		};

		Assert.DoesNotThrow(() => coll.Apply(source, target));
	}

	[Test]
	public void UseTargetAdd()
	{
		var called = 0f;
		var coll = new MockEffectCollection();
		var factory = new MockEffectFactory();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		factory.create = (s, t, _) => new Effect(maintain: d => called = d);
		target.add = e => e.Maintain(42f);
		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory, duration = 1f }
		};

		coll.Apply(source, target);

		Assert.AreEqual(42f, called);
	}

	[Test]
	public void DontUseTargetAddWhenApplyReturnsFalse()
	{
		var called = false;
		var coll = new MockEffectCollection();
		var factory = new MockEffectFactory();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		target.add = _ => called = true;
		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory }
		};

		coll.Apply(source, target);

		Assert.False(called);
	}
}
