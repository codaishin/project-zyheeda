using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BaseEffectCollectionTests : TestCollection
{
	private class MockStack : IStack
	{
		public Action<Effect> push;

		public IEnumerable<Effect> Effects => throw new NotImplementedException();
		public void Cancel() => throw new NotImplementedException();
		public void Push(Effect effect) => this.push(effect);
	}

	private class MockEffectRunner : IEffectRunner
	{
		public Func<EffectTag, ConditionStacking, IStack> getStack;
		public IStack this[EffectTag tag, ConditionStacking stacking] => getStack(tag, stacking);
	}

	private class MockSheetMB : MonoBehaviour, ISections
	{
		public Func<Delegate, Action, Action> useSection;
		public Action UseSection<T>(RefAction<T> action, Action fallback) =>
			this.useSection(action, fallback);
	}

	private class MockEffectFactory : IEffectFactory
	{
		public Func<ISections, ISections, float, Effect> create = (_, __, ___) => new Effect();

		public Effect Create<TSheet>(TSheet s, TSheet t, float i)
			where TSheet : ISections => this.create(s, t, i);
	}

	private class MockEffectCollection : BaseEffectCollection<MockSheetMB, MockEffectRunner, MockEffectFactory> {}


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
		var called = (default(EffectTag), default(ConditionStacking), default(Effect));
		var runner = new MockEffectRunner();
		var coll = new MockEffectCollection();
		var factory = new MockEffectFactory();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		var effect = new Effect{ tag = EffectTag.Heat, stacking = ConditionStacking.Duration };

		factory.create = (s, t, _) => effect;
		runner.getStack = (t, s) => new MockStack { push = e => called = (t, s, e) };
		target.useSection = (Delegate action, Action _) => action switch {
			RefAction<MockEffectRunner> use => () => use(ref runner),
			_ => null,
		};
		coll.effectData = new EffectData<MockSheetMB, MockEffectFactory>[] {
			new EffectData<MockSheetMB, MockEffectFactory> { factory = factory, duration = 1f }
		};

		coll.Apply(source, target);

		Assert.AreEqual((EffectTag.Heat, ConditionStacking.Duration, effect), called);
	}
}
