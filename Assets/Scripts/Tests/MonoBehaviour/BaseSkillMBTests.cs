using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseSkillMBTests : TestCollection
{
	private class MockCast : ICast
	{
		public delegate IEnumerator<WaitForFixedUpdate> ApplyFunc(GameObject t);

		public ApplyFunc apply = MockCast.BaseApply;

		private static IEnumerator<WaitForFixedUpdate> BaseApply(GameObject _) { yield break; }

		public IEnumerator<WaitForFixedUpdate> Apply(GameObject target) =>
			this.apply(target);
	}

	private class MockEffect : IEffectCollection<MockSheet>
	{
		public delegate bool GetEffectFunc(MockSheet s, GameObject t, out Action a);

		public GetEffectFunc getApplyEffects = MockEffect.BaseGetEffectFor;

		private static bool BaseGetEffectFor(MockSheet _, GameObject __, out Action a)
		{
			a = () => {};
			return true;
		}

		public bool GetApplyEffects(MockSheet source, GameObject target, out Action applyEffects) =>
			this.getApplyEffects(source, target, out applyEffects);
	}

	private class MockSheet : ISheet
	{
		public Attributes attributes;

		public Attributes Attributes => this.attributes;

		public int Hp { get; set; }
	}

	private class MockSkillMB : BaseSkillMB<MockEffect, MockCast, MockSheet> {}

	[UnityTest]
	public IEnumerator Begin()
	{
		var applied = false;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			applied = true;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.True(applied);
	}

	[UnityTest]
	public IEnumerator DontBeginWhenGetEffectFalse()
	{
		var got = default(GameObject);
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject t) {
			got = t;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.getApplyEffects = (MockSheet s, GameObject t, out Action applyEffects) => {
			applyEffects = () => {};
			return false;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.Null(got);
	}

	[UnityTest]
	public IEnumerator ApplyEffect()
	{
		var got = (default(MockSheet), default(GameObject));
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		skill.effectCollection.getApplyEffects = (MockSheet s, GameObject t, out Action applyEffects) => {
			applyEffects = () => got = (s, t);
			return true;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual((skill.sheet, target.gameObject), got);
	}

	[UnityTest]
	public IEnumerator DontApplyEffectWhenGetEffectFalse()
	{
		var got = (default(MockSheet), default(GameObject));
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		skill.effectCollection.getApplyEffects = (MockSheet s, GameObject t, out Action applyEffects) => {
			applyEffects = () => got = (s, t);
			return false;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.AreEqual((default(MockSheet), default(GameObject)), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast()
	{
		var got = new List<(MockSheet, GameObject)>();
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject t) {
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.getApplyEffects = (MockSheet s, GameObject t, out Action applyEffects) => {
			applyEffects = () => got.Add((s, t));
			return true;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		CollectionAssert.AreEqual(
			new (MockSheet, GameObject)[] {
				(default, target.gameObject),
				(default, target.gameObject),
				(skill.sheet, target.gameObject),
			},
			got
		);
	}

	[UnityTest]
	public IEnumerator DontBeginDuringCooldown()
	{
		var applied = 0;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 1;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);
		skill.Begin(target.gameObject);

		Assert.AreEqual(1, applied);
	}

	[UnityTest]
	public IEnumerator BeginAfterCooldown()
	{
		var applied = 0;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		yield return new WaitForSeconds(0.11f);

		skill.Begin(target.gameObject);

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator BeginWithNoCooldown()
	{
		var applied = 0;
		var target = new GameObject("target");
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(GameObject _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);
		skill.Begin(target.gameObject);

		Assert.AreEqual(2, applied);
	}
}
