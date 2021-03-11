using System;
using NUnit.Framework;
using UnityEngine;

public class BaseEffectCollectionTests : TestCollection
{
	private class MockSheetMB : MonoBehaviour, IConditionManager, ISections
	{
		public Action<Effect> add = (_) => { };

		public void Add(Effect effect) => this.add(effect);
		public void UseSection<T>(SectionAction<T> action, bool required) {}
	}

	private class MockEffectData : IEffectData
	{
		public Func<ISections, ISections, Effect> create = (s, t) => new Effect();

		public Effect GetEffect<TSheet>(TSheet source, TSheet target) where TSheet : ISections =>
			this.create(source, target);
	}

	private class MockEffectCollection : BaseEffectCollection<MockEffectData, MockSheetMB> {}

	[Test]
	public void GetApplyEffectsTrue()
	{
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		coll.effectData = new MockEffectData[0];
		Assert.True(coll.GetApplyEffects(source, target.gameObject, out _));
	}

	[Test]
	public void GetApplyEffectsFalse()
	{
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target");
		coll.effectData = new MockEffectData[0];
		Assert.False(coll.GetApplyEffects(source, target, out _));
	}

	[Test]
	public void GetApplyEffectsApply()
	{
		var called = (default(MockSheetMB), default(MockSheetMB));
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		Effect create(ISections s, ISections t) {
			Effect effect = new Effect();
			effect.OnApply += () => called = (s as MockSheetMB, t as MockSheetMB);
			return effect;
		};

		coll.effectData = new MockEffectData[] {
			new MockEffectData { create = create }
		};
		coll.GetApplyEffects(source, target.gameObject, out var apply);
		apply();

		Assert.AreEqual((source, target), called);
	}

	[Test]
	public void GetApplyEffectsRevert()
	{
		var called = (default(MockSheetMB), default(MockSheetMB));
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		Effect create(ISections s, ISections t) {
			Effect effect = new Effect();
			effect.OnRevert += () => called = (s as MockSheetMB, t as MockSheetMB);
			return effect;
		};

		coll.effectData = new MockEffectData[] {
			new MockEffectData { create = create }
		};
		coll.GetApplyEffects(source, target.gameObject, out var apply);
		apply();

		Assert.AreEqual((source, target), called);
	}
}
