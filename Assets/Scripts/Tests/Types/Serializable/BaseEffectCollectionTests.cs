using System;
using NUnit.Framework;
using UnityEngine;

public class BaseEffectCollectionTests : TestCollection
{
	private class MockSheetMB : MonoBehaviour, IConditionManager
	{
		public Action<Effect> add = (_) => { };

		public void Add(Effect effect) => this.add(effect);
	}

	private class MockEffectCreator : IEffectCreator<MockSheetMB>
	{
		public Func<MockSheetMB, MockSheetMB, Effect> create = (s, t) => new Effect();

		public Effect Create(MockSheetMB source, MockSheetMB target) =>
			this.create(source, target);
	}

	private class MockEffectCollection : BaseEffectCollection<MockEffectCreator, MockSheetMB> {}

	[Test]
	public void GetApplyEffectsTrue()
	{
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		coll.effectData = new MockEffectCreator[0];
		Assert.True(coll.GetApplyEffects(source, target.gameObject, out _));
	}

	[Test]
	public void GetApplyEffectsFalse()
	{
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target");
		coll.effectData = new MockEffectCreator[0];
		Assert.False(coll.GetApplyEffects(source, target, out _));
	}

	[Test]
	public void GetApplyEffectsApply()
	{
		var called = (default(MockSheetMB), default(MockSheetMB));
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		Effect create(MockSheetMB s, MockSheetMB t) {
			Effect effect = new Effect();
			effect.OnApply += () => called = (s, t);
			return effect;
		};

		coll.effectData = new MockEffectCreator[] {
			new MockEffectCreator { create = create }
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
		Effect create(MockSheetMB s, MockSheetMB t) {
			Effect effect = new Effect();
			effect.OnRevert += () => called = (s, t);
			return effect;
		};

		coll.effectData = new MockEffectCreator[] {
			new MockEffectCreator { create = create }
		};
		coll.GetApplyEffects(source, target.gameObject, out var apply);
		apply();

		Assert.AreEqual((source, target), called);
	}
}
