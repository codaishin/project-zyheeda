using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseSkillMBTests : TestCollection
{
	private class MockSheet { }

	private class MockTargetingSO : BaseTargetingSO<MockSheet>
	{
		public WaitForEndOfFrame[] yields = new WaitForEndOfFrame[0];
		public Action<MockSheet, List<MockSheet>, int> select = (_, __, ___) => {};

		protected override
		IEnumerator<WaitForEndOfFrame> DoSelect(MockSheet source, List<MockSheet> targets, int maxCount = 1)
		{
			this.select(source, targets, maxCount);
			foreach (var yield in this.yields) {
				yield return yield;
			}
		}
	}

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
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => targets.Add(target);

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			applied = true;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.True(applied);
	}

	[UnityTest]
	public IEnumerator TargetingFromSource()
	{
		var source = default(object);
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (s, targets, __) => {
			source = s;
			targets.Add(target);
		};

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.AreSame(skill.Sheet, source);
	}

	[UnityTest]
	public IEnumerator TargetingWithMaxCount()
	{
		var called = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.maxTargetCount = 42;
		skill.targeting = targeting;
		targeting.select = (_, targets, mC) => {
			called = mC;
			targets.Add(target);
		};

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.AreEqual(42, called);
	}

	[UnityTest]
	public IEnumerator ApplyEffect()
	{
		var got = (default(MockSheet), default(MockSheet));
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => targets.Add(target);

		skill.effectCollection.apply = (s, t) => got = (s, t);

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.AreEqual((skill.Sheet, target), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectPerTarget()
	{
		var got = new List<(MockSheet, MockSheet)>();
		var targetA = new MockSheet();
		var targetB = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => {
			targets.Add(targetA);
			targets.Add(targetB);
		};

		skill.effectCollection.apply = (s, t) => got.Add((s, t));

		yield return new WaitForEndOfFrame();

		skill.Begin();

		CollectionAssert.AreEqual(
			new (MockSheet, MockSheet)[] {
				(skill.Sheet, targetA),
				(skill.Sheet, targetB)
			},
			got
		);
	}


	[UnityTest]
	public IEnumerator ApplyEffectPerTargetParallel()
	{
		var got = string.Empty;
		var targetA = new MockSheet();
		var targetB = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => {
			targets.Add(targetA);
			targets.Add(targetB);
		};

		IEnumerator<WaitForFixedUpdate> castApply(MockSheet target) {
			got += target == targetA ? "A" : "B";
			yield return new WaitForFixedUpdate();
			got += target == targetA ? "A" : "B";
			yield return new WaitForFixedUpdate();
		}

		skill.cast.apply = castApply;

		yield return new WaitForEndOfFrame();

		skill.Begin();

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual("ABAB", got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast()
	{
		var got = new List<(MockSheet, MockSheet)>();
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => targets.Add(target);

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet t) {
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
		}

		skill.cast.apply = applyCast;
		skill.effectCollection.apply = (s, t) => got.Add((s, t));

		yield return new WaitForEndOfFrame();

		skill.Begin();

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
	public IEnumerator BeginCooldownAfterSelect()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => {
			++applied;
			targets.Add(target);
		};

		targeting.yields = new WaitForEndOfFrame[] {
			new WaitForEndOfFrame(),
		};
		skill.applyPerSecond = 1;

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator BeginCooldownOnlyWhenTargetsFound()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, __, ___) => ++applied;

		skill.applyPerSecond = 1;

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator DontBeginDuringCooldown()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => targets.Add(target);

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 1;

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(1, applied);
	}

	[UnityTest]
	public IEnumerator WaitForTargetingRoutine()
	{
		var applied = false;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.yields = new WaitForEndOfFrame[]{
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
			new WaitForEndOfFrame(),
		};

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			applied = true;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.False(applied);
	}

	[UnityTest]
	public IEnumerator BeginAfterCooldown()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => targets.Add(target);

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;
		skill.applyPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin();

		yield return new WaitForSeconds(0.11f);

		skill.Begin();

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator BeginWithNoCooldown()
	{
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;
		targeting.select = (_, targets, __) => targets.Add(target);

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		skill.cast.apply = applyCast;

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(2, applied);
	}
}
