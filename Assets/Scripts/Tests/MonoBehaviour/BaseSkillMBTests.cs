using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseSkillMBTests : TestCollection
{
	private class MockSheet { }

	private delegate IEnumerable<WaitForEndOfFrame> SelectFn(
		MockSheet sheet,
		List<MockSheet> targets,
		int maxCount
	);

	private class MockTargetingSO : BaseTargetingSO<MockSheet>
	{
		public SelectFn doSelect
			= (_, __, ___) => Enumerable.Empty<WaitForEndOfFrame>();

		protected override IEnumerable<WaitForEndOfFrame> DoSelect(
			MockSheet source,
			List<MockSheet> targets,
			int maxCount
		) => this.doSelect(source, targets, maxCount);
	}

	private class MockSkillMB : BaseSkillMB<MockSheet>
	{
		public Action<MockSheet, MockSheet> applyEffects
			= (s, t) => { };
		public Func<MockSheet, IEnumerator<WaitForFixedUpdate>> applyCast
			= _ => Enumerable.Empty<WaitForFixedUpdate>().GetEnumerator();

		protected override void ApplyEffects(
			MockSheet source,
			MockSheet target
		) => this.applyEffects(source, target);

		protected override IEnumerator<WaitForFixedUpdate> ApplyCast(
			MockSheet target
		) => this.applyCast(target);
	}

	[UnityTest]
	public IEnumerator TargetingFromSource() {
		var source = default(object);
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet targetingSource,
			List<MockSheet> _,
			int __
		) {
			source = targetingSource;
			yield break;
		};

		targeting.doSelect = selectTargets;
		skill.Sheet = new MockSheet();
		skill.targeting = targeting;

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.AreSame(skill.Sheet, source);
	}

	[UnityTest]
	public IEnumerator TargetingWithMaxCount() {
		var called = 0;
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> __,
			int mC
		) {
			called = mC;
			yield break;
		};

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.maxTargetCount = 42;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.AreEqual(42, called);
	}

	[UnityTest]
	public IEnumerator ApplyEffect() {
		var got = (default(MockSheet), default(MockSheet));
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(target);
			yield break;
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyEffects = (s, t) => got = (s, t);
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.AreEqual((skill.Sheet, target), got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectPerTarget() {
		var got = new List<(MockSheet, MockSheet)>();
		var targetA = new MockSheet();
		var targetB = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(targetA);
			targets.Add(targetB);
			yield break;
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyEffects = (s, t) => got.Add((s, t));
		skill.Sheet = new MockSheet();

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
	public IEnumerator ApplyEffectPerTargetParallel() {
		var got = string.Empty;
		var targetA = new MockSheet();
		var targetB = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(targetA);
			targets.Add(targetB);
			yield break;
		}

		IEnumerator<WaitForFixedUpdate> castApply(MockSheet target) {
			got += target == targetA ? "A" : "B";
			yield return new WaitForFixedUpdate();
			got += target == targetA ? "A" : "B";
			yield return new WaitForFixedUpdate();
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyCast = castApply;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.AreEqual("ABAB", got);
	}

	[UnityTest]
	public IEnumerator ApplyEffectAfterCast() {
		var got = new List<(MockSheet, MockSheet)>();
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(target);
			yield break;
		}

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet t) {
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
			got.Add((default, t));
			yield return new WaitForFixedUpdate();
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyCast = applyCast;
		skill.applyEffects = (s, t) => got.Add((s, t));
		skill.Sheet = new MockSheet();

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
	public IEnumerator BeginCooldownAfterSelect() {
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			++applied;
			targets.Add(target);
			yield return new WaitForEndOfFrame();
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyPerSecond = 1;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator BeginCooldownOnlyWhenTargetsFound() {
		var applied = 0;
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			++applied;
			yield break;
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyPerSecond = 1;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator DontBeginDuringCooldown() {
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(target);
			yield break;
		}

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyCast = applyCast;
		skill.applyPerSecond = 1;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(1, applied);
	}

	[UnityTest]
	public IEnumerator WaitForTargetingRoutine() {
		var applied = false;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(target);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			applied = true;
			yield break;
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyCast = applyCast;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();

		Assert.False(applied);
	}

	[UnityTest]
	public IEnumerator BeginAfterCooldown() {
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(target);
			yield break;
		}

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting;
		skill.applyCast = applyCast;
		skill.applyPerSecond = 10;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();

		yield return new WaitForSeconds(0.11f);

		skill.Begin();

		Assert.AreEqual(2, applied);
	}

	[UnityTest]
	public IEnumerator BeginWithNoCooldown() {
		var applied = 0;
		var target = new MockSheet();
		var skill = new GameObject("item").AddComponent<MockSkillMB>();
		var targeting = ScriptableObject.CreateInstance<MockTargetingSO>();

		IEnumerable<WaitForEndOfFrame> selectTargets(
			MockSheet _,
			List<MockSheet> targets,
			int __
		) {
			targets.Add(target);
			yield break;
		}

		IEnumerator<WaitForFixedUpdate> applyCast(MockSheet _) {
			++applied;
			yield break;
		}

		targeting.doSelect = selectTargets;
		skill.targeting = targeting; ;
		skill.applyCast = applyCast;
		skill.Sheet = new MockSheet();

		yield return new WaitForEndOfFrame();

		skill.Begin();
		skill.Begin();

		Assert.AreEqual(2, applied);
	}
}
