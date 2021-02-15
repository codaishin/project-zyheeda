using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ProjectileLauncherSOTests : TestCollection
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

	private SkillMB MockSkill()
	{
		var skill = new GameObject("skill").AddComponent<SkillMB>();
		var item = new GameObject("item");
		skill.transform.SetParent(item.transform);
		return skill;
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

	[Test]
	public void BaseProjectilePath()
	{
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		Assert.Fail();
	}

	[Test]
	public void CallTryHit()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __, float ___) {
			yield break;
		};

		skill.modifiers.offense = 42;
		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.AreEqual(42, target.usedOffense);
	}

	[Test]
	public void CallTryHitValid()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __, float ___) {
			yield break;
		};

		behaviour.projectileRoutine = yieldNone;
		var valid = behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.True(valid);
	}

	[Test]
	public void UseProjectilePathing()
	{
		var count = 0;
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldFive(Transform _, Transform __, float ___) {
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
		};

		behaviour.projectileRoutine = yieldFive;
		behaviour.Apply(skill, target.gameObject, out var routine);
		while (routine.MoveNext()) {
			++count;
		}
		Assert.AreEqual(5, count);
	}

	[Test]
	public void Parameters()
	{
		var got = (null as Transform, null as Transform, 0f);
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform f, Transform t, float d) {
			got = (f, t, d);
			yield break;
		};

		behaviour.deltaPerSecond = 10f;
		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.AreEqual((skill.Item.transform, target.transform, 10f), got);
	}

	[Test]
	public void NoProjectilePathing()
	{
		var target = new GameObject("target");
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __, float ___) {
			yield break;
		};

		behaviour.projectileRoutine = yieldNone;
		var valid = behaviour.Apply(skill, target, out var routine);
		Assert.AreEqual(
			(null as IEnumerator<WaitForFixedUpdate>, false),
			(routine, valid)
		);
	}

	[Test]
	public void ApplyEffects()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __, float ___) {
			yield break;
		};

		target.hit = true;
		skill.effects = effects;
		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		CollectionAssert.AreEqual(
			new (SkillMB, GameObject)[] {
				(skill, target.gameObject),
				(skill, target.gameObject),
			},
			effects.Select(e => (e.skill, e.target))
		);
	}

	[Test]
	public void DontApplyEffects()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldNone(Transform _, Transform __, float ___) {
			yield break;
		};

		skill.effects = effects;
		behaviour.projectileRoutine = yieldNone;
		behaviour.Apply(skill, target.gameObject, out var routine);
		routine.MoveNext();

		Assert.True(effects.All(e => !e.skill && !e.target));
	}

	[Test]
	public void ApplyEffectsOnlyAfterProjectileHit()
	{
		var target = new GameObject("target").AddComponent<MockHitableMB>();
		var effects = new MockEffectSO[] {
			ScriptableObject.CreateInstance<MockEffectSO>(),
			ScriptableObject.CreateInstance<MockEffectSO>(),
		};
		var applied = new List<bool>();
		var behaviour = ScriptableObject.CreateInstance<ProjectileLauncherSO>();
		var skill = this.MockSkill();
		IEnumerator<WaitForFixedUpdate> yieldTwo(Transform _, Transform __, float ___) {
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
		};

		target.hit = true;
		skill.effects = effects;
		behaviour.projectileRoutine = yieldTwo;
		behaviour.Apply(skill, target.gameObject, out var routine);
		while (routine.MoveNext()) {
			applied.Add(effects.All(e => e.skill || e.target));
		}
		applied.Add(effects.All(e => e.skill || e.target));

		CollectionAssert.AreEqual(new bool[] { false, false, true }, applied);
	}
}
