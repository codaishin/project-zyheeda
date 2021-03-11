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

	private class MockEffectBehaviourSO : BaseEffectBehaviourSO
	{
		public Action<ISections, ISections, int> apply = (s, t, i) => { };
		public Action<ISections, ISections, int, float> maintain = (s, t, i, d) => { };
		public Action<ISections, ISections, int> revert = (s, t, i) => { };

		public override
		void Apply<TSheet>(TSheet source, TSheet target, int intensity) =>
			this.apply(source, target, intensity);

		public override
		void Maintain<TSheet>(TSheet source, TSheet target, int intensity, float intervalDelta) =>
			this.maintain(source, target, intensity, intervalDelta);

		public override
		void Revert<TSheet>(TSheet source, TSheet target, int intensity) =>
			this.revert(source, target, intensity);

	}

	private class MockEffectCollection : BaseEffectCollection<MockSheetMB> {}

	[Test]
	public void GetApplyEffectsTrue()
	{
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();
		coll.effectData = new EffectData[0];
		Assert.True(coll.GetApplyEffects(source, target.gameObject, out _));
	}

	[Test]
	public void GetApplyEffectsFalse()
	{
		var coll = new MockEffectCollection();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target");
		coll.effectData = new EffectData[0];
		Assert.False(coll.GetApplyEffects(source, target, out _));
	}

	[Test]
	public void GetApplyEffectsApply()
	{
		var called = (default(MockSheetMB), default(MockSheetMB));
		var coll = new MockEffectCollection();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		behaviour.apply = (s, t, i) => called = (s as MockSheetMB, t as MockSheetMB);
		coll.effectData = new EffectData[] {
			new EffectData { behaviour = behaviour }
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
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		behaviour.apply = (s, t, i) => called = (s as MockSheetMB, t as MockSheetMB);
		coll.effectData = new EffectData[] {
			new EffectData { behaviour = behaviour }
		};
		coll.GetApplyEffects(source, target.gameObject, out var apply);
		apply();

		Assert.AreEqual((source, target), called);
	}
}
