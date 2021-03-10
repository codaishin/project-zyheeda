using System;
using NUnit.Framework;
using UnityEngine;

public class EffectDataTests : TestCollection
{
	private class MockEffectBehaviourSO : BaseEffectBehaviourSO
	{
		public Action<CharacterSheetMB, CharacterSheetMB, int> apply = (s, t, i) => { };
		public Action<CharacterSheetMB, CharacterSheetMB, int, float> maintain = (s, t, i, d) => { };
		public Action<CharacterSheetMB, CharacterSheetMB, int> revert = (s, t, i) => { };

		public override
		void Apply(CharacterSheetMB source, CharacterSheetMB target, int intensity) =>
			this.apply(source, target, intensity);

		public override
		void Maintain(CharacterSheetMB source, CharacterSheetMB target, int intensity, float intervalDelta) =>
			this.maintain(source, target, intensity, intervalDelta);

		public override
		void Revert(CharacterSheetMB source, CharacterSheetMB target, int intensity) =>
			this.revert(source, target, intensity);
	}

	[Test]
	public void CreateEffectApply()
	{
		var called = default((CharacterSheetMB, CharacterSheetMB));
		var source = new GameObject("source").AddComponent<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };
		behaviour.apply = (s, t, _) => called = (s, t);

		var effect = data.Create(source, target);
		effect.Apply();

		Assert.AreEqual((source, target), called);
	}

	[Test]
	public void CreateEffectMaintain()
	{
		var called = default((CharacterSheetMB, CharacterSheetMB, float));
		var source = new GameObject("source").AddComponent<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };
		behaviour.maintain = (s, t, _, d) => called = (s, t, d);

		var effect = data.Create(source, target);
		effect.duration = 1f;
		effect.Apply();
		effect.Maintain(0.42f);

		Assert.AreEqual((source, target, 0.42f), called);
	}

	[Test]
	public void CreateEffectRevert()
	{
		var called = default((CharacterSheetMB, CharacterSheetMB));
		var source = new GameObject("source").AddComponent<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour };
		behaviour.revert = (s, t, _) => called = (s, t);

		var effect = data.Create(source, target);
		effect.Apply();
		effect.Revert();

		Assert.AreEqual((source, target), called);
	}

	[Test]
	public void SetDuration()
	{
		var source = new GameObject("source").AddComponent<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour, duration = 42f };

		var effect = data.Create(source, target);

		Assert.AreEqual(42f, effect.duration);
	}

	[Test]
	public void UseIntensity()
	{
		var called = (a: 0, m: 0, r: 0);
		var source = new GameObject("source").AddComponent<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { behaviour = behaviour, duration = 1, intensity = 7 };

		behaviour.apply = (_, __, i) => called.a = i;
		behaviour.maintain = (_, __, i, ___) => called.m = i;
		behaviour.revert = (_, __, i) => called.r = i;

		var effect = data.Create(source, target);

		effect.Apply();
		effect.Maintain(5f);
		effect.Revert();

		Assert.AreEqual((7, 7, 7), called);
	}
}
