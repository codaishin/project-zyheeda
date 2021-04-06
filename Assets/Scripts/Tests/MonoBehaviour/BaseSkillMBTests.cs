using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseSkillMBTests : TestCollection
{
	private class MockSheet { }

	private class MockCast : ICast<MockSheet>
	{
		public Func<MockSheet, IEnumerator<WaitForFixedUpdate>> apply = MockCast.BaseApply;
		public IEnumerator<WaitForFixedUpdate> Apply(MockSheet target) => this.apply(target);
		private static IEnumerator<WaitForFixedUpdate> BaseApply(MockSheet _) { yield break; }
	}

	private class MockEffect : IEffectCollection<MockSheet>
	{
		public Action<MockSheet, MockSheet> apply = (s, t) => {};
		public void Apply(MockSheet source, MockSheet target) => this.apply(source, target);
	}

	private class MockSkillMB : BaseSkillMB<MockEffect, MockCast, MockSheet> {}

	[UnityTest]
	public IEnumerator Begin()
	{
		var applied = false;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.Sheet = new MockSheet();

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
	public IEnumerator ApplyEffect()
	{
		var got = (default(MockSheet), default(MockSheet));
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.Sheet = new MockSheet();

		skill.effectCollection.apply = (s, t) => got = (s, t);

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreEqual((skill.Sheet, target), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast()
	{
		var got = new List<(MockSheet, MockSheet)>();
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		skill.Sheet = new MockSheet();

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet t) {
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.apply = (s, t) => got.Add((s, t));

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		CollectionAssert.AreEqual(
			new (MockSheet, MockSheet)[] {
				(default, target),
				(default, target),
				(skill.Sheet, target),
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
		skill.Sheet = new MockSheet();

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
		skill.Sheet = new MockSheet();

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
		skill.Sheet = new MockSheet();

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
