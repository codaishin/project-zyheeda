using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SkillMBTests : TestCollection
{
	private class MockHitableMB : BaseHitableMB
	{
		public int usedOffense;
		public bool hit;

		public override bool TryHit(in int offense)
		{
			this.usedOffense = offense;
			return this.hit;
		}
	}

	private class MockEffectSO : BaseEffectSO
	{
		public SkillMB skill;
		public GameObject target;

		public override void Apply(in SkillMB skill, in GameObject target)
		{
			this.skill = skill;
			this.target = target;
		}
	}

	private class MockItemBehaviourSO : BaseItemBehaviourSO
	{
		public SkillMB skill;
		public GameObject target;
		public int iterationsDone;
		public int applied;
		public bool valid = true;

		public override
		bool Apply(SkillMB skill, GameObject target, out IEnumerator<WaitForFixedUpdate> routine)
		{
			IEnumerator<WaitForFixedUpdate> iterate() {
				while (this.valid && this.iterationsDone < 10) {
					++this.iterationsDone;
					yield return new WaitForFixedUpdate();
				}
			}
			this.target = target;
			this.skill = skill;
			++this.applied;
			routine = iterate();
			return this.valid;
		}
	}

	[UnityTest]
	public IEnumerator CallsCorrectTarget()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreSame(target, behaviour.target);
	}

	[UnityTest]
	public IEnumerator CallsCorrectSkill()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreSame(skill, behaviour.skill);
	}

	[UnityTest]
	public IEnumerator BehaviourRunsAsCoroutine()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		var ran = new int[2];
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		ran[0] = behaviour.iterationsDone;

		yield return new WaitForFixedUpdate();

		ran[1] = behaviour.iterationsDone;

		CollectionAssert.AreEqual(new int[] { 1, 2 }, ran);
	}

	[UnityTest]
	public IEnumerator StopCoroutine()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		var ran = new int[2];
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		ran[0] = behaviour.iterationsDone;
		skill.End();

		yield return new WaitForFixedUpdate();

		ran[1] = behaviour.iterationsDone;

		CollectionAssert.AreEqual(new int[] { 1, 1 }, ran);
	}

	[Test]
	public void ImplementesIPausableWithFixedUpdateYield()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();

		Assert.True(skill is IPausable<WaitForFixedUpdate>);
	}

	[UnityTest]
	public IEnumerator IsPausable()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		skill.Paused = true;

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(1, behaviour.iterationsDone);
	}

	[UnityTest]
	public IEnumerator CanBeUnPaused()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		skill.Paused = true;

		yield return new WaitForFixedUpdate();

		skill.Paused = false;

		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();

		Assert.Greater(behaviour.iterationsDone, 1);
	}

	[UnityTest]
	public IEnumerator NoApplyDuringCooldown()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		skill.modifiers.speedPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Begin(target);

		Assert.AreEqual(1, behaviour.applied);
	}

	[UnityTest]
	public IEnumerator CanApplyAfterCooldown()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		skill.modifiers.speedPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		yield return new WaitForSeconds(0.12f);

		skill.Begin(target);

		Assert.AreEqual(2, behaviour.applied);
	}

	[UnityTest]
	public IEnumerator CanAttemptToApplyAgainAfterInvalid()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		skill.modifiers.speedPerSecond = 0.1f;
		behaviour.valid = false;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		Assert.AreEqual(2, behaviour.applied);
	}

	[UnityTest]
	public IEnumerator CanAttemptToApplyAgainAfterCooldownLongerThanSkill()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		skill.modifiers.speedPerSecond = 10f;
		behaviour.iterationsDone = 9; // 9 out of 10 iterations done

		yield return new WaitForEndOfFrame();

		skill.Begin(target);

		yield return new WaitForSeconds(0.11f);

		skill.Begin(target);

		Assert.AreEqual(2, behaviour.applied);
	}

	[UnityTest]
	public IEnumerator NoApplyDuringCooldownWhenPaused()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		skill.modifiers.speedPerSecond = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Paused = true;

		yield return new WaitForSeconds(0.12f);

		skill.Paused = false;
		skill.Begin(target);

		Assert.AreEqual(1, behaviour.applied);
	}

	[UnityTest]
	public IEnumerator ApplyEndToAllCoroutines()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target);
		skill.Begin(target);
		skill.End();

		yield return new WaitForFixedUpdate();

		Assert.AreEqual(2, behaviour.iterationsDone);
	}

	[Test]
	public void PauseNotNull()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		Assert.NotNull(skill.Pause);
	}

	[Test]
	public void ParentIsItem()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var item = new GameObject("item");
		skill.transform.SetParent(item.transform);

		Assert.AreSame(item, skill.Item);
	}

	[UnityTest]
	public IEnumerator ApplyEffects()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();

		skill.effects = effects;
		skill.behaviour = behaviour;
		target.hit = true;
		behaviour.iterationsDone = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		CollectionAssert.AreEqual(
			new (SkillMB, GameObject)[] {
				(skill, target.gameObject),
				(skill, target.gameObject),
			},
			effects.Select(e => (e.skill, e.target))
		);
	}

	[UnityTest]
	public IEnumerator DontApplyEffects()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();

		skill.effects = effects;
		skill.behaviour = behaviour;
		behaviour.iterationsDone = 10;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		Assert.True(effects.All(e => !e.skill && !e.target));
	}

		[UnityTest]
	public IEnumerator ApplyEffectsOnlyAfterProjectileHit()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		var applied = new List<bool>();

		skill.effects = effects;
		skill.behaviour = behaviour;
		target.hit = true;
		behaviour.iterationsDone = 9;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		applied.Add(effects.All(e => e.skill || e.target));

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		applied.Add(effects.All(e => e.skill || e.target));

		CollectionAssert.AreEqual(new bool[] { false, true }, applied);
	}

	[UnityTest]
	public IEnumerator CallTryHit()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var behaviour = ScriptableObject.CreateInstance<MockItemBehaviourSO>();
		behaviour.iterationsDone = 10;
		skill.modifiers.offense = 42;
		skill.behaviour = behaviour;

		yield return new WaitForEndOfFrame();

		skill.Begin(target.gameObject);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		Assert.AreEqual(42, target.usedOffense);
	}
}
