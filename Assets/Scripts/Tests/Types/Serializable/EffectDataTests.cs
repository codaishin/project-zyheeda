using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EffectDataTests : TestCollection
{
	private class MockEffectBehaviourSO : BaseEffectBehaviourSO
	{
		public Action<CharacterSheetMB, CharacterSheetMB> onApply = (s, t) => { };
		public Action<CharacterSheetMB, CharacterSheetMB, float> onMaintain = (s, t, d) => { };
		public Action<CharacterSheetMB, CharacterSheetMB> onRevert = (s, t) => { };

		public override void Apply(CharacterSheetMB source, CharacterSheetMB target) =>
			this.onApply(source, target);
		public override void Maintain(CharacterSheetMB source, CharacterSheetMB target, float intervalDelta) =>
			this.onMaintain(source, target, intervalDelta);
		public override void Revert(CharacterSheetMB source, CharacterSheetMB target) =>
			this.onRevert(source, target);
	}

	[Test]
	public void CreateEffectApply()
	{
		var called = default((CharacterSheetMB, CharacterSheetMB));
		var source = new GameObject("source").AddComponent<CharacterSheetMB>();
		var target = new GameObject("target").AddComponent<CharacterSheetMB>();
		var behaviour = ScriptableObject.CreateInstance<MockEffectBehaviourSO>();
		var data = new EffectData { effectBehaviour = behaviour };
		behaviour.onApply = (s, t) => called = (s, t);

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
		var data = new EffectData { effectBehaviour = behaviour };
		behaviour.onMaintain = (s, t, d) => called = (s, t, d);

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
		var data = new EffectData { effectBehaviour = behaviour };
		behaviour.onRevert = (s, t) => called = (s, t);

		var effect = data.Create(source, target);
		effect.Apply();
		effect.Revert();

		Assert.AreEqual((source, target), called);
	}

	[Test]
	public void EffectTagProperty()
	{
		var data = new EffectData();
		data.effectTag = EffectTag.Heat;

		Assert.AreEqual(EffectTag.Heat, data.EffectTag);
	}
}
