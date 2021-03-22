using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseSkillMBTests : TestCollection
{
	private class MockCast : ICast<MockSheet>
	{
		public delegate IEnumerator<WaitForFixedUpdate> ApplyFunc(MockSheet t);

		public ApplyFunc apply = MockCast.BaseApply;

		private static IEnumerator<WaitForFixedUpdate> BaseApply(MockSheet _) { yield break; }

		public IEnumerator<WaitForFixedUpdate> Apply(MockSheet target) =>
			this.apply(target);
	}

	private class MockSheet { }

	private class MockEffect : IEffectCollection<MockSheet>
	{
		public delegate bool GetEffectFunc(MockSheet s, MockSheet t, out Action a);

		public GetEffectFunc getApplyEffects = MockEffect.BaseGetEffectFor;

		private static bool BaseGetEffectFor(MockSheet _, MockSheet __, out Action a)
		{
			a = () => {};
			return true;
		}

		public bool GetApplyEffects(MockSheet source, MockSheet target, out Action applyEffects) =>
			this.getApplyEffects(source, target, out applyEffects);
	}

	private class MockSkillMB : BaseSkillMB<MockEffect, MockCast, MockSheet> {}

	[UnityTest]
	public IEnumerator Begin()
	{
		var applied = false;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			applied = true;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.True(applied);
	}

	[UnityTest]
	public IEnumerator DontBeginWhenGetEffectFalse()
	{
		var got = default(MockSheet);
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet t) {
			got = t;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.getApplyEffects = (MockSheet s, MockSheet t, out Action applyEffects) => {
			applyEffects = () => {};
			return false;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.Null(got);
	}

	[UnityTest]
	public IEnumerator ApplyEffect()
	{
		var got = (default(MockSheet), default(MockSheet));
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		skill.effectCollection.getApplyEffects = (MockSheet s, MockSheet t, out Action applyEffects) => {
			applyEffects = () => got = (s, t);
			return true;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreEqual((skill.sheet, target), got);
	}

	[UnityTest]
	public IEnumerator DontApplyEffectWhenGetEffectFalse()
	{
		var got = (default(MockSheet), default(MockSheet));
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		skill.effectCollection.getApplyEffects = (MockSheet s, MockSheet t, out Action applyEffects) => {
			applyEffects = () => got = (s, t);
			return false;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreEqual((default(MockSheet), default(MockSheet)), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast()
	{
		var got = new List<(MockSheet, MockSheet)>();
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet t) {
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.getApplyEffects = (MockSheet s, MockSheet t, out Action applyEffects) => {
			applyEffects = () => got.Add((s, t));
			return true;
		};

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		CollectionAssert.AreEqual(
			new (MockSheet, MockSheet)[] {
				(default, target),
				(default, target),
				(skill.sheet, target),
			},
			got
		);
	}

	[UnityTest]
	public IEnumerator DontBeginDuringCooldown()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 1;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Begin(target);

		Assert.AreEqual(1, applied);
	}

	[UnityTest]
	public IEnumerator BeginAfterCooldown()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		yield return new WaitForSeconds(0.11f);

		skill.Begin(target);

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator BeginWithNoCooldown()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Begin(target);

		Assert.AreEqual(2, applied);
	}
}
