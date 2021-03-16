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

	private class MockEffectBehaviourSO : BaseEffectBehaviourSO
	{
		public Action<ISections, ISections, float> apply = (s, t, i) => { };
		public Action<ISections, ISections, float, float> maintain = (s, t, i, d) => { };
		public Action<ISections, ISections, float> revert = (s, t, i) => { };

		public override
		void Apply<TSheet>(TSheet source, TSheet target, float intensity) =>
			this.apply(source, target, intensity);

		public override
		void Maintain<TSheet>(TSheet source, TSheet target, float intensity, float intervalDelta) =>
			this.maintain(source, target, intensity, intervalDelta);

		public override
		void Revert<TSheet>(TSheet source, TSheet target, float intensity) =>
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

		behaviour.revert = (s, t, i) => called = (s as MockSheetMB, t as MockSheetMB);
		coll.effectData = new EffectData[] {
			new EffectData { behaviour = behaviour }
		};
		coll.GetApplyEffects(source, target.gameObject, out var apply);
		apply();

		Assert.AreEqual((source, target), called);
	}

	[Test]
	public void AddsToConditions()
	{
		var called = (default(MockSheetMB), default(MockSheetMB), 0f);
		var coll = new MockEffectCollection();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var source = new GameObject("source").AddComponent<MockSheetMB>();
		var target = new GameObject("target").AddComponent<MockSheetMB>();

		behaviour.maintain = (s, t, _, d) => called = (s as MockSheetMB, t as MockSheetMB, d);
		target.add = e => {
			e.Apply();
			e.Maintain(42f);
		};
		coll.effectData = new EffectData[] {
			new EffectData { behaviour = behaviour, duration = 1f }
		};
		coll.GetApplyEffects(source, target.gameObject, out var apply);
		apply();

		Assert.AreEqual((source, target, 42f), called);
	}
}
