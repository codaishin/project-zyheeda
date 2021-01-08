using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseEffectMBTests : TestCollection
{
	private class MockItemMB : BaseItemMB
	{
		public override
		bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
		{
			throw new System.NotImplementedException();
		}
	}

	private class MockEffectMB : BaseEffectMB
	{
		public override void Apply(in SkillMB skill, in GameObject target)
		{
			throw new System.NotImplementedException();
		}
	}

	[Test]
	public void ExposeItemMB()
	{
		var item = new GameObject("item").AddComponent<MockItemMB>();
		var effect = item.gameObject.AddComponent<MockEffectMB>();

		Assert.AreSame(item, effect.Item);
	}

	[Test]
	public void AddsItselfToItem()
	{
		var item = new GameObject("item").AddComponent<MockItemMB>();
		var effect = item.gameObject.AddComponent<MockEffectMB>();

		Assert.AreEqual(
			(true, true),
			(item.Effects.TryGetValue(effect.GetInstanceID(), out var e), e == effect)
		);
	}

	[UnityTest]
	public IEnumerator RemovesItselfOnDisable()
	{
		var item = new GameObject("item").AddComponent<MockItemMB>();
		var effect = item.gameObject.AddComponent<MockEffectMB>();

		effect.enabled = false;

		yield return new WaitForEndOfFrame();

		Assert.False(item.Effects.TryGetValue(effect.GetInstanceID(), out _));
	}

	[UnityTest]
	public IEnumerator ReaddsItselfToItem()
	{
		var item = new GameObject("item").AddComponent<MockItemMB>();
		var effect = item.gameObject.AddComponent<MockEffectMB>();

		effect.enabled = false;

		yield return new WaitForEndOfFrame();

		effect.enabled = true;

		yield return new WaitForEndOfFrame();

		Assert.AreEqual(
			(true, true),
			(item.Effects.TryGetValue(effect.GetInstanceID(), out var e), e == effect)
		);
	}
}
